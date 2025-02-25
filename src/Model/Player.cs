﻿using System;
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
using ExampleFlight.src.Statistic;
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

        public StatisticUtil statUtil;
        private float interval = 0f;

        private TestCase testCase;

        public StateModeling oldStateModel = StateModeling.None;
        public StateDriveCar oldStateDrive = StateDriveCar.FNone;

        public enum StateDriveCar
        {
            Forward,
            Backward,
            FNone,
            BNone
        }

        public enum StateModeling
        {
            Stat,
            Test,
            None
        }

        public Player(Car car)
        {
            this.car = car;
            statUtil = new StatisticUtil(car, interval);
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
            //Drive
            if (gp.RightTrigger > 0)
            {
                car.driveForward(gp.RightTrigger);
            }
            else
            {
                //car.driveIdle();
            }
            //brake
            if (gp.LeftTrigger > 0)
            {
                car.brakeLight(gp.LeftTrigger);
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

            //Neither key was pressed, so de-steer.
            if (!car.steered)
            {
                car.driveTurnIdle(gameTime);
            }

            if (gp.IsKeyPressed(GamepadButtons.Start))
            {
                car.resetCar();
                car.isImage = !car.isImage;
                oldStateDrive = StateDriveCar.FNone;
                if (oldStateModel.Equals(StateModeling.Stat) || oldStateModel.Equals(StateModeling.Test))
                {
                    statUtil.closeStreamWriter();
                    oldStateModel = StateModeling.None;
                }
                    
            }
            

            if (gp.IsKeyPressed(GamepadButtons.B))
            {
                if (oldStateDrive.Equals(StateDriveCar.FNone))
                {
                    oldStateDrive = StateDriveCar.Backward;
                    car.switchForwardBack(true);
                } 
                else if (oldStateDrive.Equals(StateDriveCar.BNone))
                {
                    oldStateDrive = StateDriveCar.Forward;
                    car.switchForwardBack(false);
                }
            }
            else
            {
                if (oldStateDrive.Equals(StateDriveCar.Forward))
                {
                    oldStateDrive = StateDriveCar.FNone;
                }
                else if (oldStateDrive.Equals(StateDriveCar.Backward))
                {
                    oldStateDrive = StateDriveCar.BNone;
                }
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

            if (gp.IsKeyPressed(GamepadButtons.Y) && oldStateModel.Equals(StateModeling.None))
            {
                oldStateModel = StateModeling.Stat;
                statUtil.openFileStream("e:\\test.txt");
            }
            if (oldStateModel.Equals(StateModeling.Stat))
            {
                statUtil.executeOperation(gameTime, StatisticUtil.StatFunction.All);
            }

            if (gp.IsKeyPressed(GamepadButtons.X) && oldStateModel.Equals(StateModeling.None))
            {
                testCase = new TestCase(car, statUtil);
                testCase.initTest();
                oldStateModel=StateModeling.Test;
            }
            if (oldStateModel.Equals(StateModeling.Test))
            {
                testCase.executeTest(gameTime);
                if (testCase.isFinished())
                {
                    oldStateModel=StateModeling.None;
                }
            }
        }

        

        //private void initKeyboard(GameTime gameTime, InputDevice device)
        //{
        //    if (device.IsKeyDown(Keys.R))
        //    {
        //        car.resetCar();
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
        //            car.driveForward(gameTime, 0.1f);
        //        }
        //        else
        //        {
        //            car.driveBack(gameTime, 0.1f);
        //        }
        //    }

        //    if (device.IsKeyDown(Keys.H))
        //    {
        //        //brake
        //        car.brakeLight(gameTime, 0.1f);
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
        //        car.driveRight(gameTime, 0.1f);
        //    }
        //    if (device.IsKeyDown(Keys.G))
        //    {
        //        car.driveLeft(gameTime, 0.1f);
        //    }
        //    if (!car.steered)
        //    {
        //        //Neither key was pressed, so de-steer.
        //        car.driveTurnIdle(gameTime);
        //    }


        //    //car.driveIdle(gameTime);
        //}
    }
}
