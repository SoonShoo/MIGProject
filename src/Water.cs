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
        }

        public void Update()
        {
            
        }

        public void  Draw()
        {
            //this.DrawModel(1.0f, );
        }

    }
}
