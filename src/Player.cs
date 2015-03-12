using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Vehicle;
using ExampleFlight.src.Model;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Development;
using Microsoft.SqlServer.Server;
using BVector3 = BEPUutilities.Vector3;
using Vector3 = Fusion.Mathematics.Vector3;

namespace ExampleFlight
{
    class Player
    {
        public Car car;

        public Player(Car car)
        {
            this.car = car;
        }

        public void Control(GameTime gameTime, InputDevice device)
        {

            Gamepad gp = device.GetGamepad(0);
            if (!gp.IsConnected)
                initKeyboard(gameTime, device);
            else
            {
                initGamePad(gameTime, gp);
            }

        }

        private void initGamePad(GameTime gameTime, Gamepad gp)
        {
           // gp.IsKeyPressed(GamepadButtons.DPadUp)
           // gp.LeftTrigger>0
            if (gp.RightTrigger > 0)
            {
                //Drive
                car.Vehicle.Wheels[0].DrivingMotor.TargetSpeed = car.ForwardSpeed;
                car.Vehicle.Wheels[2].DrivingMotor.TargetSpeed = car.ForwardSpeed;
            }
            else if (gp.LeftTrigger > 0)
            {
                //Reverse
                car.Vehicle.Wheels[0].DrivingMotor.TargetSpeed = car.BackwardSpeed;
                car.Vehicle.Wheels[2].DrivingMotor.TargetSpeed = car.BackwardSpeed;
            }
            else
            {
                //Idle
                car.Vehicle.Wheels[0].DrivingMotor.TargetSpeed = 0;
                car.Vehicle.Wheels[2].DrivingMotor.TargetSpeed = 0;
            }


            if (gp.IsKeyPressed(GamepadButtons.RightShoulder))
            {
                //Brake
                foreach (Wheel wheel in car.Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = true;

                }
            }
            else
            {
                //Release brake
                foreach (Wheel wheel in car.Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = false;
                }
            }
            //Use smooth steering; while held down, move towards maximum.
            //When not pressing any buttons, smoothly return to facing forward.
            float angle;
            bool steered = false;
            if (gp.LeftStick.X < 0)
            {
                steered = true;
                //angle = Math.Max(car.Vehicle.Wheels[1].Shape.SteeringAngle - car.config.TurnSpeed * delta, -car.config.MaximumTurnAngle);
                angle = Math.Max(gp.LeftStick.X * car.MaximumTurnAngle, -car.MaximumTurnAngle);
                car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }

            if (gp.LeftStick.X > 0)
            {
                steered = true;
                //angle = Math.Min(car.Vehicle.Wheels[1].Shape.SteeringAngle + car.config.TurnSpeed * delta, car.config.MaximumTurnAngle);
                angle = Math.Max(gp.LeftStick.X * car.MaximumTurnAngle, -car.MaximumTurnAngle);
                car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }

            if (!steered)
            {
                //Neither key was pressed, so de-steer.
                if (car.Vehicle.Wheels[1].Shape.SteeringAngle > 0)
                {
                    angle = Math.Max(car.Vehicle.Wheels[1].Shape.SteeringAngle - car.TurnSpeed * gameTime.ElapsedSec, 0);
                    car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
                else
                {
                    angle = Math.Min(car.Vehicle.Wheels[1].Shape.SteeringAngle + car.TurnSpeed * gameTime.ElapsedSec, 0);
                    car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
            }

            if (gp.IsKeyPressed(GamepadButtons.Start))
                car.SetPosition(0,0,0);
        }

        private void initKeyboard(GameTime gameTime, InputDevice device)
        {
            if (device.IsKeyDown(Keys.Y))
            {
                //Drive
                car.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = car.ForwardSpeed;
                car.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = car.ForwardSpeed;
                //Console.WriteLine(Vehicle.Wheels[1].DrivingMotor.TargetSpeed);
            }
            else if (device.IsKeyDown(Keys.H))
            {
                //Reverse
                car.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = car.BackwardSpeed;
                car.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = car.BackwardSpeed;
            }
            else
            {
                //Idle
                car.Vehicle.Wheels[1].DrivingMotor.TargetSpeed = 0;
                car.Vehicle.Wheels[3].DrivingMotor.TargetSpeed = 0;
            }
            if (device.IsKeyDown(Keys.Space))
            {
                //Brake
                foreach (Wheel wheel in car.Vehicle.Wheels)
                {
                    wheel.Brake.IsBraking = true;
                }
            }
            else
            {
                //Release brake
                foreach (Wheel wheel in car.Vehicle.Wheels)
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
                angle = Math.Max(car.Vehicle.Wheels[1].Shape.SteeringAngle - car.TurnSpeed * gameTime.ElapsedSec, -car.MaximumTurnAngle);
                car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
            if (device.IsKeyDown(Keys.G))
            {
                steered = true;
                angle = Math.Min(car.Vehicle.Wheels[1].Shape.SteeringAngle + car.TurnSpeed * gameTime.ElapsedSec, car.MaximumTurnAngle);
                car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
            }
            if (!steered)
            {
                //Neither key was pressed, so de-steer.
                if (car.Vehicle.Wheels[1].Shape.SteeringAngle > 0)
                {
                    angle = Math.Max(car.Vehicle.Wheels[1].Shape.SteeringAngle - car.TurnSpeed * gameTime.ElapsedSec, 0);
                    car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
                else
                {
                    angle = Math.Min(car.Vehicle.Wheels[1].Shape.SteeringAngle + car.TurnSpeed * gameTime.ElapsedSec, 0);
                    car.Vehicle.Wheels[1].Shape.SteeringAngle = angle;
                    car.Vehicle.Wheels[3].Shape.SteeringAngle = angle;
                }
            }
        }
    }
}
