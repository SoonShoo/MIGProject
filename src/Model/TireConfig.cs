using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUutilities;

namespace ExampleFlight.src.Model
{
    public class TireConfig
    {
        [Category("Wheel")]
        public float radius { get; set; }
        [Category("Wheel")]
        public float width { get; set; }
        [Category("WheelSuspension")]
        public float stiffnessConstant { get; set; }
        [Category("WheelSuspension")]
        public float dampingConstant { get; set; }
        [Category("WheelSuspension")]
        public float restLength { get; set; }
        [Category("WheelDrivingMotor")]
        public float gripFriction { get; set; }
        [Category("WheelDrivingMotor")]
        public float maximumForwardForce { get; set; }
        [Category("WheelDrivingMotor")]
        public float maximumBackwardForce { get; set; }
        // WheelBrake
        [Category("WheelBrake")]
        public float dynamicBrakingFrictionCoefficient { get; set; }
        [Category("WheelBrake")]
        public float staticBrakingFrictionCoefficient { get; set; }
        [Category("WheelBrake")]
        public float rollingFrictionCoefficient { get; set; }
        //WheelSlidingFriction
        [Category("WheelSlidingFriction")]
        public float dynamicCoefficient { get; set; }
        [Category("WheelSlidingFriction")]
        public float staticCoefficient { get; set; }

        public TireConfig()
        {
            this.radius = 1f;
            this.width = 0.5f;
            //WheelSuspension
            this.stiffnessConstant = 10000;
            this.dampingConstant = 5000f;
            this.restLength = 0.725f;
            // WheelDrivingMotor
            this.gripFriction = 2.5f;
            this.maximumForwardForce = 30000;
            this.maximumBackwardForce = 10000;
            // WheelBrake
            this.dynamicBrakingFrictionCoefficient = 10.5f;
            this.staticBrakingFrictionCoefficient = 20;
            this.rollingFrictionCoefficient = .02f;
            //WheelSlidingFriction
            this.dynamicCoefficient = 0.7f;
            this.staticCoefficient = 1.3f;
        }
    }
}
