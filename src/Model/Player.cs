using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Vehicle;
using BEPUutilities;
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
        public int numberCamera = 0;
        public int countCameras = 6;
        private bool flagChecking = true;
        public Player(Car car)
        {
            this.car = car;
        }

        public void Control(GameTime gameTime, InputDevice device)
        {

            Gamepad gp = device.GetGamepad(0);
            if (!gp.IsConnected)
            {
                //initKeyboard(gameTime, device);
            }
            else
            {
                initGamePad(gameTime, gp);
            }

        }

        private void initGamePad(GameTime gameTime, Gamepad gp)
        {
            
            if (gp.RightTrigger > 0)
            {
                //Drive
                if (car.isForwardRun)
                {
                    car.driveForward(gameTime, gp.RightTrigger);
                }
                else
                {
                    car.driveBack(gameTime, gp.RightTrigger);
                }
            }
            if (gp.LeftTrigger > 0)
            {
                //Reverse
                car.brakeLight(gameTime, gp.LeftTrigger);
            }
            if (gp.LeftTrigger == 0)
            {
                //Reverse
                car.unbrake();
            }

            if (gp.IsKeyPressed(GamepadButtons.RightShoulder))
            {
                //Brake
                car.brake(gameTime);
            }

            //Use smooth steering; while held down, move towards maximum.
            //When not pressing any buttons, smoothly return to facing forward.
            car.steered = false;
            if (gp.LeftStick.X < 0)
            {
                car.driveLeft(gameTime, gp.LeftStick.X);
            }

            if (gp.LeftStick.X > 0)
            {
                car.driveRight(gameTime, gp.LeftStick.X);
            }
            
            
            if (!car.steered)
            {
                //Neither key was pressed, so de-steer.
                car.driveTurnIdle(gameTime);
            }

            if (gp.IsKeyPressed(GamepadButtons.Start))
            {
                car.resetCar();
            }
            //car.driveIdle(gameTime);

            if (gp.IsKeyPressed(GamepadButtons.X))
            {
                car.isImage = true ^ car.isImage;
            }

            if (gp.IsKeyPressed(GamepadButtons.B))
            {
                car.isForwardRun = true ^ car.isForwardRun;
            }

            if (gp.IsKeyPressed(GamepadButtons.A))
            {
                var camera = numberCamera%countCameras; 
                switch(camera)
                {
                    case 0:
                        car.vecNormal = Fusion.Mathematics.Vector3.Right;
                        break;
                    case 1:
                        car.vecNormal = Fusion.Mathematics.Vector3.Left;
                        break;
                    case 2:
                        car.vecNormal = Fusion.Mathematics.Vector3.ForwardLH;
                        break;
                    case 3:
                        car.vecNormal = Fusion.Mathematics.Vector3.ForwardRH;
                        break;
                    case 4:
                        car.vecNormal = Fusion.Mathematics.Vector3.Up;
                        break;
                    case 5:
                        car.vecNormal = Fusion.Mathematics.Vector3.Down;
                        break;
                }
                numberCamera++;
            }
        }

        //private void initKeyboard(GameTime gameTime, InputDevice device)
        //{
        //    if (device.IsKeyDown(Keys.R))
        //    {
        //        car.Vehicle.Body.Position = new BVector3(10, 10, 10);
        //        car.Vehicle.Body.LinearVelocity=BVector3.Zero;
        //        car.Vehicle.Body.Orientation= Quaternion.Identity;
        //        car.SetPosition(0,0,0);

        //    }
            
        //    if (device.IsKeyDown(Keys.I))
        //    {
        //        if (flagChecking)
        //            car.isImage = true ^ car.isImage;
        //        flagChecking = false;
        //    }

        //    if (device.IsKeyDown(Keys.P))
        //    {
        //        if (flagChecking)
        //            car.isForwardRun = true ^ car.isForwardRun;
        //        flagChecking = false;
        //    }

        //    if (device.IsKeyUp(Keys.I) && device.IsKeyUp(Keys.P))
        //    {
        //        flagChecking = true;
        //    }


        //    if (device.IsKeyDown(Keys.Y))
        //    {
        //        //Drive
        //        if (car.isForwardRun)
        //        {
        //            car.driveForward(gameTime);
        //        }
        //        else
        //        {
        //            car.driveBack(gameTime);
        //        }
        //    }
            
        //    if (device.IsKeyDown(Keys.H))
        //    {
        //        //brake
        //        car.brakeLight(gameTime);
        //    }

        //    if (device.IsKeyDown(Keys.Space))
        //    {
        //        car.brake(gameTime);
        //    }
        //    else
        //    {
        //        car.unbrake();
        //    }
        //    //Use smooth steering; while held down, move towards maximum.
        //    //When not pressing any buttons, smoothly return to facing forward.
        //    car.steered = false;
        //    if (device.IsKeyDown(Keys.J))
        //    {
        //        car.driveRight(gameTime);
        //    }
        //    if (device.IsKeyDown(Keys.G))
        //    {
        //        car.driveLeft(gameTime);
        //    }
        //    if (!car.steered)
        //    {
        //        //Neither key was pressed, so de-steer.
        //        car.driveTurnIdle(gameTime);
        //    }

        //    car.driveIdle(gameTime);
        //}
    }
}
