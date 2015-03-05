using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.PositionUpdating;
using BEPUphysics.Vehicle;
using BEPUutilities;
using Fusion;
using Fusion.Graphics;
using Matrix = Fusion.Mathematics.Matrix;
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Tire : ModelOfScene
    {
        private Wheel wheel;
        private Vector3 positionBepu;


        const string modelName = "tire.fbx";
        const string shaderName = "render2"; 

        public Tire(Game game, GraphicsDevice graphicsDevice, Vector3 position)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.positionBepu = position;
            base.LoadContent(game, graphicsDevice, modelName, shaderName, 0);
            SetPosition(position.X, position.Z, position.Y);
            init();
            setScaling(0.0005f);
        }

        private void init()
        {
            var localWheelRotation = Quaternion.CreateFromAxisAngle(new Vector3(0, 0, 1), MathHelper.PiOver2);
            BEPUutilities.Matrix wheelGraphicRotation = BEPUutilities.Matrix.CreateFromAxisAngle(Vector3.Forward, MathHelper.PiOver2);
            wheel = new Wheel(
                                 new CylinderCastWheelShape(.375f, 0.2f, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(2000, 100f, Vector3.Down, 0.325f, positionBepu),
                                 new WheelDrivingMotor(2.5f, 30000, 10000),
                                 new WheelBrake(1.5f, 2, .02f),
                                 new WheelSlidingFriction(4, 5));

            wheel.Shape.FreezeWheelsWhileBraking = true;
            wheel.Suspension.SolverSettings.MaximumIterationCount = 1;
            wheel.Brake.SolverSettings.MaximumIterationCount = 1;
            wheel.SlidingFriction.SolverSettings.MaximumIterationCount = 1;
            wheel.DrivingMotor.SolverSettings.MaximumIterationCount = 1;
            wheel.DrivingMotor.GripFriction = 1;
        }

        public Wheel getWheel()
        {
            return this.wheel;
        }

        public void Update(GameTime gameTime, DebugRender dr)
        {
            SetPosition(wheel.SupportLocation.X, wheel.SupportLocation.Z, wheel.SupportLocation.Y);
            var o = wheel.Shape.LocalGraphicTransform;
            SetOrientation(new Fusion.Mathematics.Quaternion());
            dr.DrawSphere(new Fusion.Mathematics.Vector3(wheel.SupportLocation.X, wheel.SupportLocation.Z, wheel.SupportLocation.Y), 1, Fusion.Mathematics.Color.Red);
        }
    }
}
