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
        public Matrix position;
        


        public void LoadContent(Game game, GraphicsDevice graphicsDevice, string modelName, string shaderName, int fl)
        {
            SetPosition(0, 0, 0);
            this.game = game;
            this.graphicsDevice = graphicsDevice;
            this.modelName = modelName;
            this.shaderName = shaderName;
            modelConstBuffer = new ConstantBuffer(graphicsDevice, typeof(CBData));
            scene = game.Content.Load<Scene>(@modelName);
            if (fl == 1)
            {
                foreach (var mesh in scene.Meshes)
                {
                    foreach (var mtrl in mesh.Materials)
                    {
                        mtrl.Tag = this.game.Content.Load<Texture2D>(mtrl.TexturePath);
                    }
                }
            }

            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = this.game.Content.Load<Ubershader>(shaderName);
            modelUberShader.Map(typeof(RenderFlags));
            
            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }

        public enum RenderFlags
        {
            None,
        }

        public void Reload()
        {
            scene = game.Content.Load<Scene>(@modelName);
            /*	foreach ( var mesh in scene.Meshes ) {
                  foreach ( var mtrl in mesh.Materials ) {
                      mtrl.Tag	=	Content.Load<Texture2D>( mtrl.TexturePath );
                  }
              }*/

            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = this.game.Content.Load<Ubershader>(shaderName);
            modelUberShader.Map(typeof(RenderFlags));

            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }

        public void SetPosition(float x, float y, float z)
        {
            position = Matrix.Translation(x, y, z);
        }

        public void DrawModel(float scaling )
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

                    cbData.Projection = cam.ProjMatrix;
                    cbData.View = cam.ViewMatrix;
                    cbData.World = position * Matrix.RotationYawPitchRoll(j * 0.01f, j * 0.02f, j * 0.03f) * worldMatricies[i] * Matrix.Scaling((float)Math.Pow(scaling, 1));
                    cbData.ViewPos = new Vector4(cam.CameraMatrix.TranslationVector, 1);

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
                        graphicsDevice.PSShaderResources[0] = mesh.Materials[subset.MaterialIndex].Tag as Texture2D;
                        mesh.Draw(subset.StartPrimitive, subset.PrimitiveCount);
                    }
            }
        }
    }
}
