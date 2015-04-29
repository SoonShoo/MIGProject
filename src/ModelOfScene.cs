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
        public Matrix[] worldMatrixies = new Matrix[4];
        public Fusion.Mathematics.Matrix worldMatrix = Matrix.Identity;

        private Vector3 position;
        private Quaternion orientation;
        private float scaling =1;
        private Matrix rotation = Matrix.RotationYawPitchRoll(0, 0, 0);

        public bool isImage = false;
        public bool isPrint = false;

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
                    Console.WriteLine(mtrl.Name + " Tag: " + mtrl.Tag);
                    if (mtrl.TexturePath != null)
                    {
                        mtrl.Tag = this.game.Content.Load<Texture2D>(mtrl.TexturePath);
                    }
                    else
                    {
                        mtrl.Tag = this.game.Content.Load<Texture2D>("backl01.png");
                    }
		        }
            }

            scene.Bake<VertexColorTextureNormal>(graphicsDevice, VertexColorTextureNormal.Bake);

            modelUberShader = this.game.Content.Load<Ubershader>(shaderName);
            modelUberShader.Map(typeof(RenderFlags));
            
            Log.Message("{0}", scene.Nodes.Count(n => n.MeshIndex >= 0));
        }

        public void SetPosition(float x, float y, float z)
        {
            this.position = new Vector3(x, y, z);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetOrientation(Quaternion quaternion)
        {
            this.orientation = quaternion;
        }

        public void setScaling(float scaling)
        {
            this.scaling = scaling;
        }

        public void setRotation(Vector3 rotation)
        {
            this.rotation = Matrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
        }

        public Matrix getRotation()
        {
            return this.rotation;
        }

        public Quaternion getOrientation()
        {
            return this.orientation;
        }

        public Vector3 getPosition()
        {
            return this.position;
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
                if (!isPrint)
                {
                    Console.WriteLine("Name: "+node.Name+" Tag:"+node.Tag + "Matrix:" + node.Transform.TranslationVector);
                }

                var mesh = scene.Meshes[node.MeshIndex];

                cbData.Projection = cam.GetProjectionMatrix(stereoEye);
                cbData.View = cam.GetViewMatrix(stereoEye);

                //cbData.World = rotation * Matrix.AffineTransformation(scaling, orientation, position);//position * Matrix.RotationYawPitchRoll(orientation[0], 0, 0) * worldMatricies[i] * Matrix.Scaling((float)Math.Pow(scaling, 1));
                cbData.ViewPos = new Vector4(cam.GetCameraMatrix(stereoEye).TranslationVector, 1);

                if (!node.Name.Contains("tyre"))
                {
                    //TODO: change
                    cbData.World = worldMatrix;
                    //cbData.World = rotation*Matrix.AffineTransformation(scaling, orientation, position);
                    //cbData.World = worldMatrix*Fusion.Mathematics.Matrix.RotationAxis(Fusion.Mathematics.Vector3.Left, MathUtil.PiOverTwo);
                    //if(!isImage)
                       // continue;
                }
                else
                {
                    if (node.Name.Contains("tyre01"))
                    {
                        cbData.World = worldMatrixies[0];
                    }
                    else if (node.Name.Contains("tyre02"))
                    {
                        cbData.World = worldMatrixies[1];
                    }
                    else if (node.Name.Contains("tyre03"))
                    {
                        cbData.World = worldMatrixies[2];
                    }
                    else if (node.Name.Contains("tyre04"))
                    {
                        cbData.World = worldMatrixies[3];
                    }
                }
                //cbData.World = worldMatrix * Fusion.Mathematics.Matrix.RotationAxis(Fusion.Mathematics.Vector3.Left, MathUtil.PiOverTwo);
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
            isPrint = true;
        }

        public Fusion.Mathematics.Matrix switchMatrixFromBepu(BEPUutilities.Matrix bepuMatrix)
        {
            return new Fusion.Mathematics.Matrix
            {
                M11 = bepuMatrix.M11,
                M13 = bepuMatrix.M12,
                M12 = bepuMatrix.M13,
                M14 = bepuMatrix.M14,

                M21 = bepuMatrix.M21,
                M23 = bepuMatrix.M22,
                M22 = bepuMatrix.M23,
                M24 = bepuMatrix.M24,

                M31 = bepuMatrix.M31,
                M33 = bepuMatrix.M32,
                M32 = bepuMatrix.M33,
                M34 = bepuMatrix.M34,

                M41 = bepuMatrix.M41,
                M43 = bepuMatrix.M42,
                M42 = bepuMatrix.M43,
                M44 = bepuMatrix.M44,

                //Backward = switchVectorFromBepu(bepuMatrix.Backward),
                //Down = switchVectorFromBepu(bepuMatrix.Down),
                //Forward = switchVectorFromBepu(bepuMatrix.Forward),
                //Up = switchVectorFromBepu(bepuMatrix.Up),
                //Left = switchVectorFromBepu(bepuMatrix.Left),
                //Right = switchVectorFromBepu(bepuMatrix.Right),
                //TranslationVector = switchVectorFromBepu(bepuMatrix.Translation),
            };
        }

        public BEPUutilities.Matrix switchMatrixFromBepu(Fusion.Mathematics.Matrix bepuMatrix)
        {
            return new BEPUutilities.Matrix
            {
                M11 = bepuMatrix.M11,
                M12 = bepuMatrix.M12,
                M13 = bepuMatrix.M13,
                M14 = bepuMatrix.M14,

                M21 = bepuMatrix.M21,
                M22 = bepuMatrix.M22,
                M23 = bepuMatrix.M23,
                M24 = bepuMatrix.M24,

                M31 = bepuMatrix.M31,
                M32 = bepuMatrix.M32,
                M33 = bepuMatrix.M33,
                M34 = bepuMatrix.M34,

                M41 = bepuMatrix.M41,
                M42 = bepuMatrix.M42,
                M43 = bepuMatrix.M43,
                M44 = bepuMatrix.M44,

                //Backward = switchVectorFromBepu(bepuMatrix.Backward),
                //Down = switchVectorFromBepu(bepuMatrix.Down),
                //Forward = switchVectorFromBepu(bepuMatrix.Forward),
                //Up = switchVectorFromBepu(bepuMatrix.Up),
                //Left = switchVectorFromBepu(bepuMatrix.Left),
                //Right = switchVectorFromBepu(bepuMatrix.Right),
                //TranslationVector = switchVectorFromBepu(bepuMatrix.Translation),
            };
        }

        public Fusion.Mathematics.Vector3 switchVectorFromBepu(BEPUutilities.Vector3 bepuVector3)
        {
            return new Fusion.Mathematics.Vector3
            {
                X = bepuVector3.X,
                Y = bepuVector3.Z,
                Z = bepuVector3.Y
            };
        }
    }
}
