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
        //CylinderCastWheelShape
        private Vector3 positionBepu;
        private const float radius=1f;
        private const float width = 0.2f;
        //WheelSuspension
        private const float stiffnessConstant = 2000;
        private const float dampingConstant = 100f;
        private Vector3 localDirection = Vector3.Down;
        private const float restLength = 1.325f;
        private Vector3 localAttachmentPoint;
        // WheelDrivingMotor
        private const float gripFriction = 2.5f;
        private const float maximumForwardForce = 30000;
        private const float maximumBackwardForce = 10000;
        // WheelBrake
        private const float dynamicBrakingFrictionCoefficient = 1.5f;
        private const float staticBrakingFrictionCoefficient = 2;
        private const float rollingFrictionCoefficient = .02f;
        //WheelSlidingFriction
        private const float dynamicCoefficient = 1;
        private const float staticCoefficient = 1;

        public Tire(Game game, GraphicsDevice graphicsDevice, Vector3 position)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.localAttachmentPoint = position;
            SetPosition(position.X, position.Y, position.Z);
            init(position);
        }

        private void init(Vector3 position)
        {
            var localWheelRotation = Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), MathHelper.PiOver2*1);
            var wheelGraphicRotation = BEPUutilities.Matrix.CreateFromAxisAngle(new Vector3(1,0,0), MathHelper.PiOver2 * 1);
            //var wheelGraphicRotation = switchMatrixFromBepu(Matrix.AffineTransformation(1, new Fusion.Mathematics.Quaternion(new Fusion.Mathematics.Vector3(1,0,0), Fusion.Mathematics.MathUtil.PiOverTwo), switchVectorFromBepu(position)));//BEPUutilities.Matrix.CreateFromAxisAngle(new Vector3(0,0,1), MathHelper.PiOver2*0);
            wheel = new Wheel(
                new CylinderCastWheelShape(radius, width, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(stiffnessConstant, dampingConstant, localDirection, restLength, localAttachmentPoint),
                                 new WheelDrivingMotor(gripFriction, maximumForwardForce, maximumBackwardForce),
                                 new WheelBrake(dynamicBrakingFrictionCoefficient, staticBrakingFrictionCoefficient, rollingFrictionCoefficient),
                                 new WheelSlidingFriction(dynamicCoefficient, staticCoefficient));

            wheel.Shape.FreezeWheelsWhileBraking = true;
            wheel.Suspension.SolverSettings.MaximumIterationCount = 1;
            wheel.Brake.SolverSettings.MaximumIterationCount = 10;
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
            dr.DrawSphere(
                new Fusion.Mathematics.Vector3(wheel.SupportLocation.X, wheel.SupportLocation.Z, wheel.SupportLocation.Y),
                radius, Fusion.Mathematics.Color.Red);
        }
    }
}
