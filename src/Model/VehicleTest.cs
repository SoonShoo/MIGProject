using System.IO;
using BEPUphysics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BEPUphysics.Entities.Prefabs;
using Fusion;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Mathematics;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class VehicleTest : ModelOfScene
    {
        private Space space;
        private Sphere sphere;
        private float coorX = 1;
        private float coorY = 1;
        private float coorZ = 10f;
        private float radius = 1;

        public VehicleTest(Game game, Space space, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.space = space;
            this.graphicsDevice = graphicsDevice;
            Init();
        }

        private void Init()
        {
            String modelName = "";
            String shaderName = "";
            //base.LoadContent(game, this.GraphicsDevice, modelName, shaderName, 1);
            sphere = new Sphere(new Vector3(coorX, coorZ, coorY), radius, 1);
            sphere.Position = new Vector3(coorX, coorZ, coorY);
            sphere.LinearVelocity = new Vector3(4f, 0, 0);
            sphere.AngularVelocity= new Vector3(0.1f, 0, 0);
            sphere.LinearMomentum=new Vector3(0.1f, 0,0);
            space.Add(sphere);

        }

        internal void Update(GameTime gameTime, DebugRender dr, InputDevice device)
        {
            dr.DrawSphere(new Fusion.Mathematics.Vector3(sphere.Position.X, sphere.Position.Z, sphere.Position.Y), radius, Color.Red);
            if (device.IsKeyDown(Keys.Up))
            {
                sphere.LinearVelocity = new Vector3(1, 0, 0);
            }
            if (device.IsKeyDown(Keys.Down))
            {
                sphere.LinearVelocity = new Vector3(-1, 0, 0);
            }
            if (device.IsKeyDown(Keys.Left))
            {
                sphere.LinearVelocity = new Vector3(0, -1, 0);
            }
            if (device.IsKeyDown(Keys.Right))
            {
                sphere.LinearVelocity = new Vector3(0, 1, 0);
            }
            if (device.IsKeyDown(Keys.Space))
            {
                sphere.Position = new Vector3(coorX, coorZ, coorY);
            }
        }


    }
}
