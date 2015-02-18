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
    class SkyBox
    {
        private ModelOfScene skyBox;
        // типы пгодных условий
        //...

        public SkyBox(Game game, GraphicsDevice graphicsDevice)
        {
            skyBox = new ModelOfScene();
            skyBox.LoadContent(game, graphicsDevice, "scenes/cube" , "render2",  1);
            game.Reloading += (s, e) => skyBox.Reload();
            skyBox.SetPosition(0.0f, 0.05f, 0.0f);
        }

        public void Update()
        {
            
        }

        public void  Draw()
        {
            skyBox.DrawModel(10.0f);
        }
    }
}
