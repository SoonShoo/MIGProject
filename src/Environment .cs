using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class Environment
    {
        Water water;
        SkyBox skyBox;
        Game game;
        World world;
        GraphicsDevice graphicsDevice;

        public Environment(Game game, World world, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            water = new Water(game, graphicsDevice);
            skyBox = new SkyBox();
            this.world = world;
        }
        public void Init()
        {
           
        }

        public void UpdatePhysics(GameTime gameTime)
        {
            //Physics of plane
            //...
        }

        public void Update(GameTime gameTime)
        {
            //Update plane
            //...
            //var scaling = 0.3f;
            UpdatePhysics(gameTime);
        }

        public void Draw(GameTime gameTime, StereoEye stereoEye)
        {
            //Update plane
            //...
            //var scaling = 0.3f;
           water.Draw();
           // skyBox.Draw();
        }
    }
}
