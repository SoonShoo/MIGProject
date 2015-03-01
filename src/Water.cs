using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics.UpdateableSystems;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Development;
using Fusion.UserInterface;
using Microsoft.SqlServer.Server;
using BVector3 = BEPUutilities.Vector3;
using Vector3 = Fusion.Mathematics.Vector3;

namespace ExampleFlight
{
    class Water : ModelOfScene
    {
        public Water(Game game, GraphicsDevice graphicsDevice)
        {
            this.LoadContent(game, graphicsDevice, "water" , "render2",  0);
            game.Reloading += (s, e) => this.Reload();
            //water = new ModelOfScene();
           // water.LoadContent(game, graphicsDevice, "scenes/water" , "render2",  1);
            //game.Reloading += (s, e) => water.Reload();
            //water.SetPosition(0.0f, -15.0f, 0.0f);
        }

        public void Update()
        {
            
        }

        public void  Draw()
        {

        }

    }
}
