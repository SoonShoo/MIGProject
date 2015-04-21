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
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Car : ModelOfScene
    {
        private float scaling = 1f;
        public  float MaxBackwardSpeed = -5f;
        public  float MaxForwardSpeed = 20f;
        public float MaximumTurnAngle = (float)Math.PI / 6;
        public float TurnSpeed = MathHelper.Pi;
        public Vehicle Vehicle;

        private const float widthCar = 5f;
        private const float lengthCar = 9f;
        private const float heightCar = 1f;

        private const float masseCar = 100;

        private float brakeCoeff = 0.50f;
        private float brakeGas = 0.70f;
        
        private float time;
        private float angle;
        public bool steered = false;

        const string modelName = "alfa_ansi.fbx";
        const string shaderName = "render2";

        private List<Tire> tires = new List<Tire>();

        private List<Vector3> wheelpositionList;

        public Car(Game game, GraphicsDevice grDevice, Vector3 position)
        {
            base.LoadContent(game, grDevice, modelName, shaderName, 1);
            SetPosition(position.X, position.Z, position.Y);
            wheelpositionList = new List<Vector3>()
            {
                new Vector3(-widthCar/2+0.5f + position.X, heightCar*0.1f + position.Z, lengthCar/2 - 0.2f + position.Y),
                new Vector3(-widthCar/2+0.5f + position.X, heightCar*0.1f + position.Z, -lengthCar/2 + 0.2f + position.Y),
                new Vector3(widthCar/2-0.5f + position.X, heightCar*0.1f + position.Z, lengthCar/2  - 0.2f + position.Y),
                new Vector3(widthCar/2-0.5f + position.X, heightCar*0.1f + position.Z, -lengthCar/2 + 0.2f + position.Y)
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
            body.CollisionInformation.LocalPosition = new Vector3(position.X, position.Z, position.Y + heightCar);
            Vehicle = new Vehicle(body);

            foreach (Vector3 pos in wheelpositionList)
            {
                Tire t = new Tire(game, this.graphicsDevice, pos);
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
            worldMatrix = switchMatrixFromBepu(Vehicle.Body.WorldTransform);
           // TODO: change WorlTransform
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.W, o.X, o.Y, o.Z));

            for (int i = 0; i < tires.Count; i++)
            {
                tires[i].Update(gameTime, dr);
                //worldMatrixies[i] = Fusion.Mathematics.Matrix.Identity;
                // TODO: change world transfrom
               // worldMatrixies[i] = switchMatrixFromBepu(Vehicle.Wheels[i].Shape.WorldTransform);
                 // worldMatrixies[i] = Fusion.Mathematics.Matrix.AffineTransformation(scaling, Fusion.Mathematics.Quaternion.One, switchVectorFromBepu(tires[i].getWheel().SupportLocation));
                //worldMatrixies[i] = switchMatrixFromBepu(Matrix.CreateFromAxisAngle(new Vector3(0, 0, -1), tires[i].getWheel().Shape.SteeringAngle / 2) * Matrix.CreateFromAxisAngle(Vector3.Left, Fusion.Mathematics.MathUtil.PiOverTwo));
                worldMatrixies[i] = Fusion.Mathematics.Matrix.RotationAxis(Fusion.Mathematics.Vector3.ForwardLH,
                    tires[i].getWheel().Shape.SteeringAngle/2 + MathHelper.Pi);
                // * Fusion.Mathematics.Matrix.AffineTransformation(1, Fusion.Mathematics.Quaternion.Identity, switchVectorFromBepu(tires[i].getWheel().SupportLocation));
                //switchMatrixFromBepu(tires[i].getWheel().Shape.WorldTransform);
                //worldMatrixies[i].Invert()
            }
            dr.DrawSphere(new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y), heightCar, Color.Red);
        }

        public void draw(StereoEye stereoEye)
        {
            //TODO: 
            this.DrawModel(stereoEye);
            //foreach(Tire t in tires)
              //  t.DrawModel(stereoEye);
        }


        public void driveForward(GameTime gameTime)
        {
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed >= 0)
            {
                if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < this.MaxForwardSpeed)
                {
                    if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 1)
                    {
                        time = gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1 / 3);
                        this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 2);
                        this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 2);
                    }
                    else
                    {
                        time = gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1);
                        this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
                        this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
                    }
                }
            }
            else
            {
                //brake
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            }
        }

        public void driveBack(GameTime gameTime)
        {
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed <= 0)
            {
                if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed > this.MaxBackwardSpeed)
                {
                    time = -gameTime.ElapsedSec + (float)Math.Pow((double)this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed, 1);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = (float)Math.Pow(time, 1);
                }
            }
            else
            {
                // brake
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed -= (float)Math.Pow(gameTime.ElapsedSec, brakeGas);
            }
        }

        public void brake(GameTime gameTime)
        {
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
            else
            {
                foreach (var wheel in this.Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = true;
                }
            }
        }

        public void unbrake()
        {
            foreach (Wheel wheel in this.Vehicle.Wheels)
            {
                wheel.Brake.IsBraking = false;
            }
        }

        public void driveIdle(GameTime gameTime)
        {
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


        public void driveLeft(GameTime gameTime)
        {
            steered = true;
            angle = Math.Min(this.Vehicle.Wheels[1].Shape.SteeringAngle + this.TurnSpeed * gameTime.ElapsedSec, this.MaximumTurnAngle);
            this.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
            this.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            
        }

        public void driveRight(GameTime gameTime)
        {
            steered = true;
            angle = Math.Max(this.Vehicle.Wheels[1].Shape.SteeringAngle - this.TurnSpeed * gameTime.ElapsedSec, -this.MaximumTurnAngle);
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
    }
}