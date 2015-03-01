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
    class ModelOfScene
    {
        public Scene scene;
        public ConstantBuffer modelConstBuffer;
        public Ubershader modelUberShader;
        public Game game;
        public string modelName;
        public string shaderName;
        public GraphicsDevice graphicsDevice;
        public Matrix[] worldMatrixies;
        protected Matrix position;
        private Vector3 vec_position;
        private Quaternion orientation;
        public float scaling = 0.2f;


        public void LoadContent(Game game, GraphicsDevice graphicsDevice, string modelName, string shaderName, int fl)
        {
            SetPosition(0, 0, 0);
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.modelName = modelName;
            this.shaderName = shaderName;
            modelConstBuffer = new ConstantBuffer(graphicsDevice, typeof(CBData));
            scene = this.game.Content.Load<Scene>(@modelName);
            if (fl == 1)
            {
                foreach ( var mtrl in scene.Materials ) {
			        mtrl.Tag	=	this.game.Content.Load<Texture2D>( mtrl.TexturePath );
		        }
            }

            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = this.game.Content.Load<Ubershader>(shaderName);
            modelUberShader.Map(typeof(RenderFlags));

            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }
          public void SetPosition(float x, float y, float z)
          {
              vec_position = new Vector3(x, y, z);
             position = Matrix.Translation(x, y, z); 
         }
          public void SetOrientation(Quaternion quaternion)
          {
              this.orientation = quaternion;
          } 
        public enum RenderFlags
        {
            None,
        }

        public void Reload()
        {
            scene = game.Content.Load<Scene>(@modelName);
            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = this.game.Content.Load<Ubershader>(shaderName);
            modelUberShader.Map(typeof(RenderFlags));

            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }

        public void DrawModel(StereoEye stereoEye)
        {
            CBData cbData = new CBData();
            var cam = game.GetService<Camera>();

            modelUberShader.SetPixelShader(0);
            modelUberShader.SetVertexShader(0);

            var worldMatricies = new Matrix[scene.Nodes.Count];
            scene.CopyAbsoluteTransformsTo(worldMatricies);
            var j = 1;
            for (int i = 0; i < scene.Nodes.Count; i++)
            {

                var node = scene.Nodes[i];

                if (node.MeshIndex == -1)
                {
                    continue;
                }

                var mesh = scene.Meshes[node.MeshIndex];

                cbData.Projection = cam.GetProjectionMatrix(stereoEye);
                cbData.View = cam.GetViewMatrix(stereoEye);
                cbData.World = Matrix.AffineTransformation(scaling, orientation, vec_position);//position * Matrix.RotationYawPitchRoll(orientation[0], 0, 0) * worldMatricies[i] * Matrix.Scaling((float)Math.Pow(scaling, 1));
                cbData.ViewPos = new Vector4(cam.GetCameraMatrix( stereoEye ).TranslationVector, 1);

                modelConstBuffer.SetData(cbData);

                graphicsDevice.RasterizerState = RasterizerState.CullNone;
                graphicsDevice.DepthStencilState = DepthStencilState.Default;
                graphicsDevice.BlendState = BlendState.Opaque;
                graphicsDevice.PSConstantBuffers[0] = modelConstBuffer;
                graphicsDevice.VSConstantBuffers[0] = modelConstBuffer;
                graphicsDevice.PSSamplerStates[0] = SamplerState.AnisotropicWrap;

                mesh.SetupVertexInput();

                foreach (var subset in mesh.Subsets)
                {
                    this.graphicsDevice.PSShaderResources[0] = scene.Materials[subset.MaterialIndex].Tag as Texture2D;
                    mesh.Draw(subset.StartPrimitive, subset.PrimitiveCount);
                }
            }
        }
    }
}
