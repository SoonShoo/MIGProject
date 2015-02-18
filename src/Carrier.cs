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
using Vector3 = Fusion.Vector3;

namespace ExampleFlight
{
    class Carrier
    {
        Game game;
        ModelOfScene carrier;
        World world;
        //GameTime gameTime;
        GraphicsDevice graphicsDevice;

        public Carrier(Game game, World world, GraphicsDevice graphicsDevice)
        {
            string modelName = "scenes/carrier";
            string shaderName = "render2";
            carrier = new ModelOfScene();
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            carrier = new ModelOfScene();
            carrier.LoadContent(game, graphicsDevice, modelName, shaderName, 1);
            this.game.Reloading += (s, e) => carrier.Reload();
            this.world = world;
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

        public void Draw(GameTime gameTime)
        {
            //Update plane
            //...
            //var scaling = 0.3f;
            carrier.DrawModel(1.0f);
         }
    }
}
