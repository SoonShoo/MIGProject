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
    class Environment : ModelOfScene
    {
        Game game;
        GraphicsDevice graphicsDevice;

        const string shaderName = "render2";

        public Environment(Game game, GraphicsDevice graphicsDevice, float scale)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            base.LoadContent(game, graphicsDevice, "field", shaderName, 1);
            this.setScaling(scale);
            game.Reloading += (s, e) => this.Reload();
            this.SetPosition(0.0f, 0.05f, 0.0f);
            worldMatrix = Fusion.Mathematics.Matrix.Identity * Fusion.Mathematics.Matrix.AffineTransformation(scale, Fusion.Mathematics.Quaternion.Zero, getPosition());
        }
    }
}
