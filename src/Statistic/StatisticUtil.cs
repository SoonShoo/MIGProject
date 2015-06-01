using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExampleFlight.src.Model;
using Fusion;
using Fusion.Mathematics;

namespace ExampleFlight.src.Statistic
{
    class StatisticUtil
    {

        private FileStream fileStream;
        private StreamWriter writer;
        private Boolean isWriting;
        private Car car;
        private float interval;
        private float time = 0;
        private float globalTime = 0;

        public enum StatFunction
        {
            CoorXY,
            SpeedTime
        }
        public StatisticUtil(Car car, float interval)
        {
            this.car = car;
            this.interval = interval;
        }

        public void executeOperation(GameTime gameTime, StatFunction function)
        {
            globalTime += gameTime.ElapsedSec;
            if (Math.Abs(time) < 0.0001)
            {
                runnnerOperation(function);
                time += gameTime.ElapsedSec;
            }
            else
            {
                time += gameTime.ElapsedSec;
                if (time > interval)
                {
                    time = 0;
                }
            }

        }

        private void runnnerOperation(StatFunction function)
        {
            switch (function)
            {
                case StatFunction.CoorXY:
                    printLineCoorXY();
                    break;
                case StatFunction.SpeedTime:
                    printLineSpeedTime();
                    break;
            }
                
        }

        private void printLineSpeedTime()
        {
            writer.Write( globalTime + ";" + car.Vehicle.Body.LinearVelocity.Length()*3.6 + "\n");
        }

        public void printLineCoorXY()
        {
            writer.Write(car.Vehicle.Body.Position.X + ";" + car.Vehicle.Body.Position.Y + "\n");
            car.computeAngle(Vector3.Right);
            var speed = car.Vehicle.Body.LinearVelocity;
            speed.Normalize();
            var additionalPoint = (car.Vehicle.Body.Position + speed) * 2;

            // writer.Write(additionalPoint.X + ";" + additionalPoint.Y + "\n");
        }

        public void openFileStream(String filename)
        {
            fileStream = new FileStream(filename, FileMode.Create);
            writer = new StreamWriter(fileStream);
            isWriting = true;
            Console.WriteLine("Create file for statistic");
        }

        public void closeStreamWriter()
        {
            isWriting = false;
            writer.Close();
            Console.WriteLine("Close file for statistic");
        }

        public Boolean getIsWriting()
        {
            return isWriting;
        }

       
    }
}
