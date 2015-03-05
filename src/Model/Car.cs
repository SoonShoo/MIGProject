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

        const string modelName = "CAR_iso2.fbx";
        const string shaderName = "render2";

        private List<Tire> tires = new List<Tire>();

        List<Vector3> wheelpositionList = new List<Vector3>()
        {
            new Vector3(-2.1f, -0.1f, 1.8f),
            new Vector3(-2.1f, -0.1f, -4f),
            new Vector3(2.1f, -0.1f, 1.8f),
            new Vector3(2.1f, -0.1f, -4f)
        };

        public Car(Game game, GraphicsDevice grDevice, Vector3 position)
        {
            base.LoadContent(game, grDevice, modelName, shaderName, 0);
            SetPosition(position.X, position.Y, position.Z);
            worldMatrix = Fusion.Mathematics.Matrix.Translation(position.X, position.Y, position.Z);
            initPhysics(position);
            setScaling(0.01f);
            setRotation(new Fusion.Mathematics.Vector3(0, (float)Math.PI, (float)-Math.PI / 2));
        }

        private void initPhysics(Vector3 position)
        {
            var bodies = new List<CompoundShapeEntry>
                {
                    new CompoundShapeEntry(new BoxShape(5f, .75f, 9f), new Vector3(0, 0, 0), 60),
                    new CompoundShapeEntry(new BoxShape(5f, .3f, 4f), new Vector3(0, .75f / 2 + .3f / 2, .5f), 1)
                };
            var body = new CompoundBody(bodies, 61);
            body.CollisionInformation.LocalPosition = new Vector3(0, .5f, 0);
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
            SetOrientation(new Fusion.Mathematics.Quaternion(o.X, o.Y, o.Z, o.W));
        }

        public void AddToSpace(Space space)
        {
            space.Add(Vehicle);
        }

        public void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            SetPosition(Vehicle.Body.Position.X, Vehicle.Body.Position.Z, Vehicle.Body.Position.Y);
            var o = Vehicle.Body.Orientation;
            SetOrientation(new Fusion.Mathematics.Quaternion(o.W, o.X, o.Y, o.Z));

            foreach (var t in tires)
            {
                t.Update(gameTime, dr);
            }
            
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

        public void draw(StereoEye stereoEye)
        {
            this.DrawModel(stereoEye);
            foreach(Tire t in tires)
                t.DrawModel(stereoEye);
        }
    }
}