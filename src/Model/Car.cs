using System;
using System.Collections.Generic;
using System.Linq;
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
//using Matrix = BEPUutilities.Matrix;
using Fusion.Mathematics;
using Matrix = BEPUutilities.Matrix;
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Car : ModelOfScene
    {
        public float BackwardSpeed = -1;
        public float ForwardSpeed = 1;
        public float MaximumTurnAngle = (float)Math.PI / 6;
        public float TurnSpeed = MathHelper.Pi;
        public Vehicle Vehicle;
        public Fusion.Mathematics.Matrix worldMatrix;

        const string modelName = "scenes/mig29.fbx";
        const string shaderName = "render2"; 

        public Car(Game game, GraphicsDevice grDevice, Vector3 position)
        {
            base.LoadContent(game, grDevice, modelName, shaderName, 1);
            SetPosition(position.X, position.Y, position.Z);
            worldMatrix = Fusion.Mathematics.Matrix.Translation(position.X, position.Y, position.Z);
            initPhysics(position);
        }

        private void initPhysics(Vector3 position)
        {
            var bodies = new List<CompoundShapeEntry>
                {
                    new CompoundShapeEntry(new BoxShape(2.5f, .75f, 4.5f), new Vector3(0, 0, 0), 60),
                    new CompoundShapeEntry(new BoxShape(2.5f, .3f, 2f), new Vector3(0, .75f / 2 + .3f / 2, .5f), 1)
                };
            var body = new CompoundBody(bodies, 61);
            body.CollisionInformation.LocalPosition = new Vector3(0, .5f, 0);
            Vehicle = new Vehicle(body);

            var localWheelRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.PiOver2);

            //The wheel model used is not aligned initially with how a wheel would normally look, so rotate them.
            Matrix wheelGraphicRotation = Matrix.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);
            Vehicle.AddWheel(new Wheel(
                                 new CylinderCastWheelShape(.375f, 0.2f, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(2000, 100f, Vector3.Down, 0.325f, new Vector3(-1.1f, -0.1f, 1.8f)),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5)));
            Vehicle.AddWheel(new Wheel(
                                 new CylinderCastWheelShape(.375f, 0.2f, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(2000, 100f, Vector3.Down, 0.325f, new Vector3(-1.1f, -0.1f, -1.8f)),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5)));
            Vehicle.AddWheel(new Wheel(
                                 new CylinderCastWheelShape(.375f, 0.2f, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(2000, 100f, Vector3.Down, 0.325f, new Vector3(1.1f, -0.1f, 1.8f)),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5)));
            Vehicle.AddWheel(new Wheel(
                                 new CylinderCastWheelShape(.375f, 0.2f, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(2000, 100f, Vector3.Down, 0.325f, new Vector3(1.1f, -0.1f, -1.8f)),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5)));


            foreach (Wheel wheel in Vehicle.Wheels)
            {
                wheel.Shape.FreezeWheelsWhileBraking = true;
                wheel.Suspension.SolverSettings.MaximumIterationCount = 1;
                wheel.Brake.SolverSettings.MaximumIterationCount = 1;
                wheel.SlidingFriction.SolverSettings.MaximumIterationCount = 1;
                wheel.DrivingMotor.SolverSettings.MaximumIterationCount = 1;
                wheel.DrivingMotor.GripFriction = 1;
            }

            for (int k = 0; k < 4; k++)
            {
                Vehicle.Wheels[k].Shape.Detector.Tag = "noDisplayObject";
            }
            Vehicle.Body.Position = position;
            Vehicle.Body.LinearVelocity = Vector3.Zero;
            Vehicle.Body.AngularVelocity = Vector3.Zero;
            Vehicle.Body.Orientation = Quaternion.Identity;
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.X, o.Y, o.Z, o.W));
        }

        public void AddToSpace(Space space)
        {
            space.Add(Vehicle);
        }

        public void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            
            //Update the wheel's graphics.
           //for (int k = 0; k < 4; k++)
           // {
           //     //WheelModels[k].WorldTransform = Vehicle.Wheels[k].Shape.WorldTransform;
           // }
            var j = 1;
            SetPosition(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y);
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.X, o.X, o.Y, o.Z));
            
            dr.DrawSphere(new Fusion.Mathematics.Vector3(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y), 1, Color.Red);

            if (device.IsKeyDown(Keys.Y))
            {
                //Drive
                Vehicle.Wheels[1].DrivingMotor.TargetSpeed = ForwardSpeed;
                Vehicle.Wheels[3].DrivingMotor.TargetSpeed = ForwardSpeed;
                Console.WriteLine(Vehicle.Wheels[1].DrivingMotor.TargetSpeed);
            }
            else if (device.IsKeyDown(Keys.H))
            {
                //Reverse
                Vehicle.Wheels[1].DrivingMotor.TargetSpeed = BackwardSpeed;
                Vehicle.Wheels[3].DrivingMotor.TargetSpeed = BackwardSpeed;
            }
            else
            {
                //Idle
                Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
                Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            }
            if (device.IsKeyDown(Keys.Space))
            {
                //Brake
                foreach (Wheel wheel in Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = true;
                }
            }
            else
            {
                //Release brake
                foreach (Wheel wheel in Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = false;
                }
            }
            //Use smooth steering; while held down, move towards maximum.
            //When not pressing any buttons, smoothly return to facing forward.
            float angle;
            bool steered = false;
            if (device.IsKeyDown(Keys.J))
            {
                steered = true;
                angle = Math.Max(Vehicle.Wheels[1].Shape.SteeringAngle - TurnSpeed * gameTime.ElapsedSec, -MaximumTurnAngle);
                Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
            if (device.IsKeyDown(Keys.G))
            {
                steered = true;
                angle = Math.Min(Vehicle.Wheels[1].Shape.SteeringAngle + TurnSpeed * gameTime.ElapsedSec, MaximumTurnAngle);
                Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
            if (!steered)
            {
                //Neither key was pressed, so de-steer.
                if (Vehicle.Wheels[1].Shape.SteeringAngle > 0)
                {
                    angle = Math.Max(Vehicle.Wheels[1].Shape.SteeringAngle - TurnSpeed * gameTime.ElapsedSec, 0);
                    Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
                else
                {
                    angle = Math.Min(Vehicle.Wheels[1].Shape.SteeringAngle + TurnSpeed * gameTime.ElapsedSec, 0);
                    Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
            }
        }
    }
}