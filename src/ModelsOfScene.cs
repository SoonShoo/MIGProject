using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using Fusion;
using Fusion.Audio;
using Fusion.Content;
using Fusion.Graphics;
using Fusion.Input;
using System.Runtime.InteropServices;
using GraphicsDevice = Fusion.Graphics.GraphicsDevice;
using Matrix = Fusion.Matrix;
using SpriteBatch = Fusion.Graphics.SpriteBatch;
using Vector3BEPU = BEPUutilities.Vector3;
using Fusion.Development;

namespace MIGProject
{
    class ModelsOfScene
    {
        public Scene scene;
        public ConstantBuffer modelConstBuffer;
        public Ubershader modelUberShader;
        public Matrix worldMatrix;
        public Texture2D texture;
        public GraphicsDevice graphicsDevice;
        public Game game;

        public ModelsOfScene(Game game, GraphicsDevice grDevice, string modelName, string shaderName, string textureName) 
        {
            //scene = game.Content.Load<Scene>(modelName);
            modelUberShader = game.Content.Load<Ubershader>(shaderName);
            graphicsDevice = grDevice;
            modelConstBuffer = new ConstantBuffer(graphicsDevice, typeof(ModelConstData));
            modelUberShader.Map(typeof(RenderFlags));
            this.game = game;
           // texture = game.Content.Load<Texture2D>(textureName);
          
		}

        public void LoadContent()
        {
            scene = game.Content.Load<Scene>(@"mig29");

            /*	foreach ( var mesh in scene.Meshes ) {
                    foreach ( var mtrl in mesh.Materials ) {
                        mtrl.Tag	=	Content.Load<Texture2D>( mtrl.TexturePath );
                    }
                }*/

            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = game.Content.Load<Ubershader>("render");
            modelUberShader.Map(typeof(RenderFlags));

            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }

        public  enum RenderFlags
        {
            None,
        }

        public void SetPosition(float x, float y, float z)
        {
            worldMatrix = Matrix.Translation(x, y, z);
        }

        public void DrawModel(Game game, GameTime gameTime, float scaling)
        {
            ModelConstData cbData = new ModelConstData();
            var cam = game.GetService<Camera>();

            graphicsDevice.ClearBackbuffer(Color.CornflowerBlue, 1, 0);
            modelUberShader.SetPixelShader(0);
            modelUberShader.SetVertexShader(0);

            var worldMatricies = new Matrix[scene.Nodes.Count];
           // Log.Message(cam.ViewMatrix.ToString());
           // Log.Message(cam.CameraMatrix.TranslationVector.ToString());
            scene.CopyAbsoluteTransformsTo(worldMatricies);

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
                //cbData.World = Matrix.Scaling(scaling) * worldMatricies[i] * worldMatrix;
                var j = 0;
                //cbData.World = Matrix.RotationYawPitchRoll(j * 0.01f, j * 0.02f, j * 0.03f) * worldMatricies[i] * Matrix.Scaling((float)Math.Pow(0.9, j));
                cbData.World = Matrix.Scaling(scaling) * Matrix.RotationYawPitchRoll(j * 0.01f, j * 0.02f, j * 0.03f) * worldMatricies[i];
                //
               // cbData.World = worldMatricies[i] * worldMatrix;
                cbData.ViewPos = new Fusion.Vector4(cam.CameraMatrix.TranslationVector, 1);

                //Log.LogInfo(cam.CameraMatrix.TranslationVector.ToString());
                modelConstBuffer.SetData(cbData);

                //grPlayer.texture = Content.Load<Texture2D>("dalek.jpg");
              /*  graphicsDevice.SetRasterizerState(RasterizerState.CullNone);
                graphicsDevice.SetDepthStencilState(DepthStencilState.Default);
                graphicsDevice.SetBlendState(BlendState.Opaque);
                graphicsDevice.SetPSConstant(0, modelConstBuffer);
                graphicsDevice.SetVSConstant(0, modelConstBuffer);
                graphicsDevice.SetPSResource(0, texture);*/
               // graphicsDevice.SetPSResource(1, texturesdfsdf);
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
    // function, change parametrs in world ( anolog disposse)
        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (modelConstBuffer != null)
                {
                    modelConstBuffer.Dispose();
                }
            }
            Dispose(disposing);
        }
    }
    
}
