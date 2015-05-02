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
    class Tire : GameService
    {
        [Config]
        public TireConfig tireConfig { get; set; }

        private Wheel wheel;
         
        //CylinderCastWheelShape
        private Vector3 positionBepu;
        private Vector3 localDirection = Vector3.Down;
        private Vector3 localAttachmentPoint;

        public Tire(Game game)
            : base(game)
        {
            tireConfig = new TireConfig();
        }

        public Tire(Game game, Vector3 position) : base(game)
        {
            tireConfig = new TireConfig();
            localAttachmentPoint = position;
            init(position);
        }

        private void init(Vector3 position)
        {
            var localWheelRotation = Quaternion.CreateFromAxisAngle(new Vector3(0,0,1), MathHelper.PiOver2*1);
            var wheelGraphicRotation = BEPUutilities.Matrix.CreateFromAxisAngle(new Vector3(1,0,0), MathHelper.PiOver2 * 1);
            //var wheelGraphicRotation = switchMatrixFromBepu(Matrix.AffineTransformation(1, new Fusion.Mathematics.Quaternion(new Fusion.Mathematics.Vector3(1,0,0), Fusion.Mathematics.MathUtil.PiOverTwo), switchVectorFromBepu(position)));//BEPUutilities.Matrix.CreateFromAxisAngle(new Vector3(0,0,1), MathHelper.PiOver2*0);
            wheel = new Wheel(
                new CylinderCastWheelShape(tireConfig.radius, tireConfig.width, localWheelRotation, wheelGraphicRotation, false),
                                 new WheelSuspension(tireConfig.stiffnessConstant, tireConfig.dampingConstant, localDirection, tireConfig.restLength, localAttachmentPoint),
                                 new WheelDrivingMotor(tireConfig.gripFriction, tireConfig.maximumForwardForce, tireConfig.maximumBackwardForce),
                                 new WheelBrake(tireConfig.dynamicBrakingFrictionCoefficient, tireConfig.staticBrakingFrictionCoefficient, tireConfig.rollingFrictionCoefficient),
                                 new WheelSlidingFriction(tireConfig.dynamicCoefficient, tireConfig.staticCoefficient));

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
            dr.DrawSphere(
                new Fusion.Mathematics.Vector3(wheel.SupportLocation.X, wheel.SupportLocation.Z, wheel.SupportLocation.Y),
                tireConfig.radius, Fusion.Mathematics.Color.Red);
            this.Update(gameTime);
        }
    }
}
