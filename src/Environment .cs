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
using Fusion.Mathematics;
using Microsoft.SqlServer.Server;
using BVector3 = BEPUutilities.Vector3;
using Vector3 = Fusion.Mathematics.Vector3;

namespace ExampleFlight
{
    class Environment : ModelOfScene
    {
        Game game;
        GraphicsDevice graphicsDevice;
        Texture2D texLoading;

        public Environment(Game game, GraphicsDevice graphicsDevice, float scale)
        {
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.modelName = "scenes/floor_new";
            this.shaderName = "render2";
            base.LoadContent(game, graphicsDevice, modelName, shaderName, 1);

            texLoading = this.game.Content.Load<Texture2D>("Textures/asphalt_47");
            this.setScaling(scale);
            game.Reloading += (s, e) => this.Reload();
            this.SetPosition(0.0f, 0.05f, 0.0f);
            worldMatrix = Fusion.Mathematics.Matrix.Identity * Fusion.Mathematics.Matrix.AffineTransformation(scale, Fusion.Mathematics.Quaternion.Zero, getPosition());
        }

        public void draw()
        {
            base.DrawModel(StereoEye.Mono);
            
        }
    }
}
