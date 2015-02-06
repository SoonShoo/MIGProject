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
    class Aircraft
    {

        Game game;
        ModelOfScene plane;
        World world;
        //GameTime gameTime;
        GraphicsDevice graphicsDevice;

        // тут поля связанные с физикой
        // ...
        // конец физических полей 

        public Aircraft(Game game, World world, GraphicsDevice graphicsDevice)
        {
            string modelName = "mig29";
            string shaderName = "render2";
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            plane = new ModelOfScene();
            plane.LoadContent(game, graphicsDevice, modelName, shaderName, 0);
            this.game.Reloading += (s, e) => plane.Reload();
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
            plane.DrawModel(0.07f);
        }
    }
}
