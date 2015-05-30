using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using BEPUphysics;
using ExampleFlight;
using Fusion;
using Fusion.Graphics;
using Fusion.Input;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.Vehicle;
using BEPUutilities;
using BEPUphysics.CollisionShapes.ConvexShapes;
using BEPUphysics.CollisionShapes;
using Fusion.Mathematics;
using Matrix = BEPUutilities.Matrix;
using Matrix3x3 = BEPUutilities.Matrix3x3;
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Car : ModelOfScene
    {
        private float scaling = 1f;

        public Vehicle Vehicle;
        const string modelName = "alfa_ansi.fbx";
        const string shaderName = "render2";

        private const float widthCar = 5f;
        private const float lengthCar = 8f;
        private const float heightCar = 1f;

        private const float masseCar = 1000;
        
        public  float MaxForwardSpeed = 50f;
        public float MaxBackwardSpeed = -10f;
        public float MaximumTurnAngle = (float)Math.PI / 18;
        public float TurnSpeed = MathHelper.Pi;
        public bool steered = false;

        public bool isForwardRun = true;

        private float brakeCoeff = 0.50f;
        private float brakeGas = 0.70f;
        
        private float time;
        private float angle;

        //camera
        float cosAlfa;
        float sinAlfa;
        public Fusion.Mathematics.Vector3 vecNormal = Fusion.Mathematics.Vector3.Right;
        private const float radius = 40;
        private const float beta = MathHelper.PiOver2 / 3;

        private List<Tire> tires = new List<Tire>();

        private List<Vector3> wheelpositionList;

        public Car(Game game, GraphicsDevice grDevice, Vector3 position)
        {
            base.LoadContent(game, grDevice, modelName, shaderName, 1);
            wheelpositionList = new List<Vector3>()
            {
                new Vector3(-widthCar/2+0.2f, heightCar*0.1f, lengthCar/2),
                new Vector3(-widthCar/2+0.2f, heightCar*0.1f, -lengthCar/2),
                new Vector3(widthCar/2-0.5f, heightCar*0.1f, lengthCar/2),
                new Vector3(widthCar/2-0.5f, heightCar*0.1f, -lengthCar/2)
            };
            //worldMatrix = Fusion.Mathematics.Matrix.Translation(position.X, position.Y, position.Z);
            initPhysics(position);
            setScaling(scaling);
            setRotation(new Fusion.Mathematics.Vector3(Fusion.Mathematics.MathUtil.PiOverTwo, 0, Fusion.Mathematics.MathUtil.PiOverTwo));
        }

        private void initPhysics(Vector3 position)
        {
            // TODO: return toc complex body!
            //var bodies = new List<CompoundShapeEntry>
            //    {
            //        new CompoundShapeEntry(new BoxShape(widthCar, heightCar*0.7f, lengthCar), position, masseCar/3/2),
            //        new CompoundShapeEntry(new BoxShape(widthCar, heightCar*.3f, lengthCar*0.5f), new Vector3(position.X, position.Z + .75f / 2 + .3f / 2, position.Y + .5f), masseCar/3)
            //    };
            //var body = new CompoundBody(bodies, masseCar);
            Box body = new Box(position, widthCar, heightCar * 0.7f, lengthCar, masseCar);
            body.CollisionInformation.LocalPosition = new Vector3(0, 0, 0);
            Vehicle = new Vehicle(body);

            foreach (Vector3 pos in wheelpositionList)
            {
                Tire t = new Tire(game, pos);
                tires.Add(t);
                Vehicle.AddWheel(t.getWheel());
            }

            Vehicle.Body.Position = position;
            Vehicle.Body.LinearVelocity = Vector3.Zero;
            Vehicle.Body.AngularVelocity = Vector3.Zero;
            Vehicle.Body.Orientation = Quaternion.Identity;
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.W, o.X, o.Y, o.Z));
        }

        public void AddToSpace(Space space)
        {
            space.Add(Vehicle);
        }

        public void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            SetPosition(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y);
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.X, o.Y, o.Z, o.W));
            worldMatrix = Fusion.Mathematics.Matrix.RotationX(MathHelper.PiOver2)
                          *Fusion.Mathematics.Matrix.RotationZ(MathHelper.PiOver2*2)
                          *Fusion.Mathematics.Matrix.Identity*switchMatrixFromBepu(Vehicle.Body.WorldTransform)
                          *Fusion.Mathematics.Matrix.Translation(0,1.5f,0);
            updateCamera(gameTime, vecNormal);
            for (int i = 0; i < tires.Count; i++)
            {
                tires[i].Update(gameTime, dr);
                //worldMatrixies[i] = Fusion.Mathematics.Matrix.Identity * switchMatrixFromBepu(tires[i].getWheel().Shape.WorldTransform);
                var rotation = Fusion.Mathematics.Matrix.RotationX( tires[i].getWheel().Shape.SteeringAngle/2);
                worldMatrixies[i] = worldMatrix; // * rotation;
                //worldMatrixies[i] = rotation * getRotation() * Fusion.Mathematics.Matrix.AffineTransformation(scaling, getOrientation(), getPosition());
            }

            var posMin = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X - widthCar/2, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y - lengthCar/2);
            var posMax = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X + widthCar/2, Vehicle.Body.Position.Z+heightCar, Vehicle.Body.Position.Y + lengthCar/2);
            dr.DrawBox(new Fusion.Mathematics.BoundingBox(posMin, posMax), Color.Red);
            dr.DrawSphere(new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y), heightCar, Color.Red);
        }

        public void draw(StereoEye stereoEye)
        {
            this.DrawModel(stereoEye);
        }


        public void driveForward(GameTime gameTime, float rightTrigger)
        {
            // rightTrigger  > 0 && < 1
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < this.MaxForwardSpeed)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += rightTrigger/10;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += rightTrigger/10;
            }
            //if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed >= 0)
            //{
            //    if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < this.MaxForwardSpeed)
            //    {
            //        if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 1)
            //        {
            //            time = gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1 / 3);
            //            this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 2);
            //            this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 2);
            //        }
            //        else
            //        {
            //            time = gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1);
            //            this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
            //            this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
            //        }
            //    }
            //}
            //else
            //{
            //    //brake
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            //}
        }

        public void driveBack(GameTime gameTime, float rightTrigger)
        {
            this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed -= rightTrigger/10;
            this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed -= rightTrigger/10;
            //if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed <= 0)
            //{
            //    //this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = -3;
            //    //this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = -3;
            //    if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > this.MaxBackwardSpeed)
            //    {
            //        time = -gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1);
            //        this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
            //        this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
            //    }
            //}
            //else
            //{
            //    // brake
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            //}
        }
        public void brakeLight(GameTime gameTime, float leftTrigger)
        {
            //this.Vehicle.Wheels[1].Brake.KineticBrakingFrictionCoefficient += leftTrigger;
            this.Vehicle.Wheels[1].Brake.RollingFrictionCoefficient = leftTrigger;
            this.Vehicle.Wheels[3].Brake.RollingFrictionCoefficient = leftTrigger;
            this.Vehicle.Wheels[0].Brake.RollingFrictionCoefficient = leftTrigger;
            this.Vehicle.Wheels[2].Brake.RollingFrictionCoefficient = leftTrigger;

            this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = Vehicle.Body.LinearVelocity.Length();
            this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = Vehicle.Body.LinearVelocity.Length();

            //this.Vehicle.Wheels[1].Brake.StaticBrakingFrictionCoefficient += leftTrigger;
            //if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed >= 0)
            //{
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed -= leftTrigger;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed -= leftTrigger;
            //    if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0)
            //    {
            //        this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
            //        this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            //    }

            //}
            //else
            //{
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += leftTrigger;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += leftTrigger;
            //    if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > 0)
            //    {
            //        this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
            //        this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            //    }
            //}
        }
        public void brake(GameTime gameTime)
        {
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed>0 && this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0.001 ||
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed<0 && this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > -0.001)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            }

            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > 0)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeCoeff);
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeCoeff);
            }
            else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeCoeff);
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeCoeff);
            }

        }

        public void unbrake()
        {
            this.Vehicle.Wheels[1].Brake.RollingFrictionCoefficient = this.tires[1].tireConfig.rollingFrictionCoefficient;
            this.Vehicle.Wheels[3].Brake.RollingFrictionCoefficient = this.tires[3].tireConfig.rollingFrictionCoefficient;
            this.Vehicle.Wheels[0].Brake.RollingFrictionCoefficient = this.tires[0].tireConfig.rollingFrictionCoefficient;
            this.Vehicle.Wheels[2].Brake.RollingFrictionCoefficient = this.tires[2].tireConfig.rollingFrictionCoefficient;
        }

        public void driveIdle(GameTime gameTime)
        {
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > 0 && this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0.001 ||
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0 && this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > -0.001)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            }
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > 0)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += -gameTime.ElapsedSec / 2;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += -gameTime.ElapsedSec / 2;
            }
            else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 0)
            {
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += gameTime.ElapsedSec / 2;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += gameTime.ElapsedSec / 2;
            }
        }


        public void driveLeft(GameTime gameTime, float leftTriggerX)
        {
            steered = true;
            leftTriggerX /= 10;
            angle = Math.Min(this.Vehicle.Wheels[1].Shape.SteeringAngle - leftTriggerX * gameTime.ElapsedSec, this.MaximumTurnAngle);
            this.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
            this.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            
        }

        public void driveRight(GameTime gameTime, float leftTriggerX)
        {
            steered = true;
            leftTriggerX /= 10;
            angle = Math.Max(this.Vehicle.Wheels[1].Shape.SteeringAngle - leftTriggerX * gameTime.ElapsedSec, -this.MaximumTurnAngle);
            this.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
            this.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
        }


        public void driveTurnIdle(GameTime gameTime)
        {
            if (this.Vehicle.Wheels[1].Shape.SteeringAngle > 0)
            {
                angle = Math.Max(this.Vehicle.Wheels[1].Shape.SteeringAngle - this.TurnSpeed * gameTime.ElapsedSec, 0);
                this.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                this.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
            else
            {
                angle = Math.Min(this.Vehicle.Wheels[1].Shape.SteeringAngle + this.TurnSpeed * gameTime.ElapsedSec, 0);
                this.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                this.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
        }

        public void resetCar()
        {
            this.Vehicle.Body.Position = new Vector3(10, 100, 100);
            this.Vehicle.Body.LinearVelocity = Vector3.Zero;
            this.Vehicle.Body.Orientation = new Quaternion(1, 0, 0, MathHelper.PiOver2);
            foreach (var tire in this.tires)
            {
                tire.updateParameters();
            }
        }

        public void updateCamera(GameTime gameTime, Fusion.Mathematics.Vector3 viewCamera)
        {
            var cam = this.game.GetService<Camera>();
            var velocity = Vehicle.Body.LinearVelocity;

            if (velocity.LengthSquared() > 0.01)
            {
                var multi = viewCamera * switchVectorFromBepu(velocity);
                cosAlfa = -(multi.X + multi.Y + multi.Z) / (viewCamera.Length() * velocity.Length());
                if (velocity.Y > 0)
                    sinAlfa = -(float)Math.Sin(Math.Acos(-cosAlfa));
                else
                    sinAlfa = (float)Math.Sin(Math.Acos(-cosAlfa));
            }
            else
            {
                cosAlfa = (float)Math.Cos(beta);
                sinAlfa = (float)Math.Sin(beta);
            }
            float x = radius * cosAlfa * (float)Math.Sin(beta);
            float y = radius * sinAlfa * (float)Math.Sin(beta);
            float z = radius * (float)Math.Sin(beta);

            Fusion.Mathematics.Vector3 thirdPersonReference = new Fusion.Mathematics.Vector3(x + Vehicle.Body.Position.X, z + Vehicle.Body.Position.Z, y + Vehicle.Body.Position.Y);
            cam.LookAt(thirdPersonReference, switchVectorFromBepu(Vehicle.Body.Position), Fusion.Mathematics.Vector3.Up);

            cam.Update(gameTime);
        }
    }
}