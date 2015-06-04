using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
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

        private float engineSpeed=500;
        private float addSpeed = 0;
        private TransmitionState transmitionState = TransmitionState.First;
        private TransmitionModel transmitionModel = TransmitionModel.AT;

        private const float maxEngineSpeed = 10000;
        private const float Step1max = 3500;
        private const float Step2max = 3200;
        private const float Step3max = 3000;
        private const float Step4max = 2700;
        private const float Step5max = 2500;

        private const float Step1min = 1000;
        private const float Step2min = 1200;
        private const float Step3min = 1500;
        private const float Step4min = 1700;
        private const float Step5min = 1850;

        public enum TransmitionState
        {
            First,
            Second,
            Third,
            Fourth,
            Fifth,
            Back
        }

        public enum TransmitionModel
        {
            MT,
            AT
        }

        private const float widthCarDown = 6f;
        private const float lengthCarDown = 14f;
        private const float heightCarDown = 1.8f;

        private const float widthCarTop = 6f;
        private const float lengthCarTop = 8f;
        private const float heightCarTop = 1.5f;

        private const float masseCar = 1500;
        
        public  float MaxForwardSpeed = 40f;
        public float MaxBackwardSpeed = -10f;
        public float MaximumTurnAngle = (float)Math.PI / 6;
        public float TurnSpeed = MathHelper.Pi;
        public bool steered = false;

        public bool isForwardRun = true;

        private float brakeCoeff = 0.50f;
        private float brakeGas = 0.70f;
        
        private float time;
        private float angle;

        //camera
        public float cosAlfa;
        public float sinAlfa;
        public Fusion.Mathematics.Vector3 vecNormal = Fusion.Mathematics.Vector3.Right;
        private const float radius = 40;
        private const float beta = MathHelper.PiOver2 / 3;

        // Transmission


        private List<Tire> tires = new List<Tire>();

        private List<Vector3> wheelpositionList;

        public Car(Game game, GraphicsDevice grDevice, Vector3 position)
        {
            base.LoadContent(game, grDevice, modelName, shaderName, 1);
            wheelpositionList = new List<Vector3>()
            {
                new Vector3(-widthCarDown/2+0.02f, heightCarDown*0.1f, lengthCarDown/2-0.01f),
                new Vector3(-widthCarDown/2+0.02f, heightCarDown*0.1f, -lengthCarDown/2+0.01f),
                new Vector3(widthCarDown/2-0.01f, heightCarDown*0.1f, lengthCarDown/2-0.01f),
                new Vector3(widthCarDown/2-0.01f, heightCarDown*0.1f, -lengthCarDown/2+0.01f)
            };
            //worldMatrix = Fusion.Mathematics.Matrix.Translation(position.X, position.Y, position.Z);
            initPhysics();
            setScaling(scaling);
            SetPosition(switchVectorFromBepu(position));
            setRotation(new Fusion.Mathematics.Vector3(Fusion.Mathematics.MathUtil.PiOverTwo, 0, Fusion.Mathematics.MathUtil.PiOverTwo));
        }

        private void initPhysics()
        {
            var bodies = new List<CompoundShapeEntry>
                {
                    new CompoundShapeEntry(new BoxShape(widthCarDown, heightCarDown, lengthCarDown), new Vector3(0,0,0), masseCar/3/2),
                    new CompoundShapeEntry(new BoxShape(widthCarTop, heightCarTop, lengthCarTop), new Vector3(0,0, heightCarDown), masseCar/3)
                };
            var body = new CompoundBody(bodies, masseCar);
            //Box body = new Box(position, widthCarDown, heightCarDown * 0.7f, lengthCarDown, masseCar);
            body.CollisionInformation.LocalPosition = new Vector3(0, 0, 0);
            Vehicle = new Vehicle(body);
            foreach (Vector3 pos in wheelpositionList)
            {
                Tire t = new Tire(game, pos);
                tires.Add(t);
                Vehicle.AddWheel(t.getWheel());
            }

            Vehicle.Body.Position = new Vector3(0, 0, 0);
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
            var ds = game.GetService<DebugStrings>();
            ds.Add(Color.Orange, "Speed {0} km/h", this.Vehicle.Body.LinearVelocity.Length()*3.6);
            ds.Add(Color.Orange, "StepTransmition {0} km/h", this.transmitionState);
            SetPosition(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y);
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.X, o.Y, o.Z, o.W));
            worldMatrix = Fusion.Mathematics.Matrix.RotationX(MathHelper.PiOver2)
                          *Fusion.Mathematics.Matrix.RotationZ(MathHelper.PiOver2*2)
                          *Fusion.Mathematics.Matrix.Identity*switchMatrixFromBepu(Vehicle.Body.WorldTransform)
                          *Fusion.Mathematics.Matrix.Translation(0,1.6f,0);
            
            for (int i = 0; i < tires.Count; i++)
            {
                //tires[i].Update(gameTime, dr);
                //worldMatrixies[i] = Fusion.Mathematics.Matrix.Identity * switchMatrixFromBepu(tires[i].getWheel().Shape.WorldTransform);
                var rotation = Fusion.Mathematics.Matrix.RotationX( tires[i].getWheel().Shape.SteeringAngle/2);
                worldMatrixies[i] = worldMatrix; // * rotation;
                //worldMatrixies[i] = rotation * getRotation() * Fusion.Mathematics.Matrix.AffineTransformation(scaling, getOrientation(), getPosition());
            }

            var posMinDown = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X - widthCarDown / 2, Vehicle.Body.Position.Z-0.5f, Vehicle.Body.Position.Y - lengthCarDown / 2);
            var posMaxDown = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X + widthCarDown / 2, Vehicle.Body.Position.Z + heightCarDown, Vehicle.Body.Position.Y + lengthCarDown / 2);
            dr.DrawBox(new Fusion.Mathematics.BoundingBox(posMinDown, posMaxDown), Color.Red);

            var posMinTop = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X - widthCarTop / 2 , Vehicle.Body.Position.Z + heightCarDown, Vehicle.Body.Position.Y - lengthCarTop / 2 - 1.5f);
            var posMaxTop = new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X + widthCarTop / 2 , Vehicle.Body.Position.Z + heightCarDown + heightCarTop, Vehicle.Body.Position.Y + lengthCarTop / 2 - 1.5f);
            dr.DrawBox(new Fusion.Mathematics.BoundingBox(posMinTop, posMaxTop), Color.Red);

            dr.DrawSphere(new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y), heightCarDown, Color.Red);


            updateCamera(gameTime, vecNormal);
        }

        public void draw(StereoEye stereoEye)
        {
            this.DrawModel(stereoEye);
        }

        public void driveForward(float rightTrigger)
        {
            // rightTrigger  > 0 && < 1
            //if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 6)
            //{
            //    var addSpeed = (float)Math.Pow((double)rightTrigger/30, (double) 1.1f);
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            //}
            //else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 15)
            //{
            //    var addSpeed = (float) Math.Pow((double) rightTrigger/25, (double) 1.3f);
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            //}
            //else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 27)
            //{
            //    var addSpeed = (float)Math.Pow((double)rightTrigger /20, (double)1.5);
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            //}
            //else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 35)
            //{
            //    var addSpeed = (float)Math.Pow((double)rightTrigger/20 , (double)1.7f);
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            //}
            //else if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < this.MaxForwardSpeed)
            //{
            //    var addSpeed = (float)Math.Pow((double)rightTrigger/15, (double)1.9);
            //    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
            //    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            //}


            
            if (transmitionModel.Equals(TransmitionModel.AT))
            {
                if (transmitionState.Equals(TransmitionState.First))
                {
                    engineSpeed += rightTrigger * 4;
                    if (engineSpeed > Step1max)
                    {
                        transmitionState = TransmitionState.Second;
                        engineSpeed = Step2min - 200;
                    }
                    addSpeed = countAddSpeed(0.015f, 2, 10000000, Step1min);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
                }

                if (transmitionState.Equals(TransmitionState.Second))
                {
                    engineSpeed += rightTrigger * 3;
                    if (engineSpeed > Step2max)
                    {
                        transmitionState = TransmitionState.Third;
                        engineSpeed = Step3min-200;
                    }
                    addSpeed = countAddSpeed(0.01f, 2, 10000000, Step2min);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
                }

                if (transmitionState.Equals(TransmitionState.Third))
                {
                    engineSpeed += rightTrigger * 2;
                    if (engineSpeed > Step3max)
                    {
                        transmitionState = TransmitionState.Fourth;
                        engineSpeed = Step4min - 200;
                    }
                    addSpeed = countAddSpeed(0.007f, 2, 10000000, Step3min);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
                }

                if (transmitionState.Equals(TransmitionState.Fourth))
                {
                    engineSpeed += rightTrigger * 1;
                    if (engineSpeed > Step4max)
                    {
                        transmitionState = TransmitionState.Fifth;
                        engineSpeed = Step5min - 200;
                    }
                    addSpeed = countAddSpeed(0.005f, 2, 10000000, Step4min);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
                }

                if (transmitionState.Equals(TransmitionState.Fifth))
                {
                    engineSpeed += rightTrigger /2;
                    addSpeed = countAddSpeed(0.004f, 2, 10000000, Step5min);
                    this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                    this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
                }
            }
        }

        private float countAddSpeed(float maxValue, float pow, float del1, float stepmin)
        {
            return addSpeed = (float)(maxValue * Math.Exp(-Math.Pow(engineSpeed - stepmin, pow) / del1));
        }
        public void driveBack(GameTime gameTime, float rightTrigger)
        {
            if (this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed < 6)
            {
                addSpeed = (float)Math.Pow((double)rightTrigger / 20, (double)1.1f);
                this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed += addSpeed;
                this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed += addSpeed;
            }
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
        public void brakeLight(float leftTrigger)
        {
            this.Vehicle.Wheels[1].Brake.RollingFrictionCoefficient = (float) Math.Pow(leftTrigger, 0.5) * 0.7f;
            this.Vehicle.Wheels[3].Brake.RollingFrictionCoefficient = (float) Math.Pow(leftTrigger, 0.5) * 0.7f;
            this.Vehicle.Wheels[0].Brake.RollingFrictionCoefficient = (float) Math.Pow(leftTrigger, 0.5) * 0.7f;
            this.Vehicle.Wheels[2].Brake.RollingFrictionCoefficient = (float) Math.Pow(leftTrigger, 0.5) * 0.7f;

            this.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = Vehicle.Body.LinearVelocity.Length();
            this.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = Vehicle.Body.LinearVelocity.Length();
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
            this.Vehicle.Body.Position = new Vector3(0, 0, 0);
            this.Vehicle.Body.LinearVelocity = Vector3.Zero;
            this.Vehicle.Body.Orientation = new Quaternion(1, 0, 0, MathHelper.PiOver2);
            this.Vehicle.Body.LinearVelocity= Vector3.Zero;
            //foreach (var tire in this.tires)
            //{
            //    tire.updateParameters();
            //}
        }

        public void updateCamera(GameTime gameTime, Fusion.Mathematics.Vector3 viewCamera)
        {
            var cam = this.game.GetService<Camera>();
            computeAngle(viewCamera);
            float x = radius * cosAlfa * (float)Math.Sin(beta);
            float y = radius * sinAlfa * (float)Math.Sin(beta);
            float z = radius * (float)Math.Sin(beta);
            Fusion.Mathematics.Vector3 thirdPersonReference = new Fusion.Mathematics.Vector3(x + Vehicle.Body.Position.X, z + Vehicle.Body.Position.Z, y + Vehicle.Body.Position.Y);
            cam.LookAt(thirdPersonReference, switchVectorFromBepu(Vehicle.Body.Position), Fusion.Mathematics.Vector3.Up);

            cam.Update(gameTime);
        }

        public void computeAngle(Fusion.Mathematics.Vector3 viewCamera)
        {
            var velocity = Vehicle.Body.LinearVelocity;

            if (velocity.LengthSquared() > 0.01)
            {
                var multi = viewCamera*switchVectorFromBepu(velocity);
                cosAlfa = -(multi.X + multi.Y + multi.Z)/(viewCamera.Length()*velocity.Length());
                if (velocity.Y > 0)
                    sinAlfa = -(float) Math.Sin(Math.Acos(-cosAlfa));
                else
                    sinAlfa = (float) Math.Sin(Math.Acos(-cosAlfa));
            }
            else
            {
                cosAlfa = (float) Math.Cos(beta);
                sinAlfa = (float) Math.Sin(beta);
            }
        }

        public float getEngineSpeed()
        {
            return engineSpeed;
        }
        public float getAddSpeed()
        {
            return addSpeed;
        }
    }
}