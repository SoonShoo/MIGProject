using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleFlight.src.Model;
using Fusion;

namespace ExampleFlight.src.Statistic
{
    class TestCase
    {
        private StatisticUtil statUtil;
        private Car car;
        private StateTesting stateTest;

        public enum StateTesting
        {
            None,
            Speed,
            Brake, 
            Finish,
        }

        public TestCase(Car car, StatisticUtil statisticUtil)
        {
            this.statUtil = statisticUtil;
            this.car = car;
            this.stateTest = StateTesting.None;
        }

        public void initTest()
        {
            Console.WriteLine("Start Test!");
            statUtil.openFileStream("e:\\testSpeed.txt");
        }

        public void executeTest(GameTime gameTime)
        {
            statUtil.executeOperation(gameTime, StatisticUtil.StatFunction.SpeedTime);
            testSpeed();
        }

        private void testSpeed()
        {
            car.driveForward(1f);
            if (car.Vehicle.Body.LinearVelocity.Length() + 1 > car.MaxForwardSpeed)
            {
                finish();
            }
        }

        private void testBrake()
        {
            car.driveForward(1f);
            if (car.Vehicle.Body.LinearVelocity.Length() + 1 > car.MaxForwardSpeed)
            {
                stateTest = StateTesting.Brake;
            }
            if (stateTest.Equals(StateTesting.Brake))
            {
                car.brakeLight(1);
                if (car.Vehicle.Body.LinearVelocity.Length() < 0.01)
                {
                    finish();
                }
            }
        }

        private void finish()
        {
            statUtil.closeStreamWriter();
            stateTest = StateTesting.Finish;
            Console.WriteLine("End Test!");
            car.resetCar();
        }
        public bool isFinished()
        {
            if (stateTest.Equals(StateTesting.Finish))
            {
                return true;
            }
            return false;
        }
    }
}
