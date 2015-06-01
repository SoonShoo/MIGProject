using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.EntityStateManagement;
using BEPUutilities;
using Fusion;
using Fusion.Graphics;
using Fusion.Mathematics;
using BoundingBox = Fusion.Mathematics.BoundingBox;
using Matrix = BEPUutilities.Matrix;
using Quaternion = BEPUutilities.Quaternion;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Field : ModelOfScene
    {
        private Space space;
        private float coorX=10000;
        private float coorY=10000;
        private float coorZ=5f;
        private Box downBox;
        private Box rightBox;
        private Box leftBox;
        private Box forwardBox;
        private Box backBox;

        const string modelName = "field.fbx";
        const string shaderName = "render2";

        private float heightCylinder=100;
        private static float radiousCylinder=0.25f;
        private const int countObjectTrace=300;
        private const float distance = 30;
        private List<Cylinder> objectTrace;


        //===============================
        // TODO: tramplin
        //private ConvexHull traplin;
        //private static float tramplinLength = 100;
        //private static float tramplinWidth = 100;
        //private static float tramplinHeight = 0;

        //List<Vector3> tramplinVector3List = new List<Vector3>()
        //    {
        //        new Vector3(-tramplinWidth/2, 0, 0),
        //        new Vector3(tramplinWidth/2, 0, 0),
        //        new Vector3(tramplinWidth/2, tramplinLength, 10),
        //        new Vector3(-tramplinWidth/2, tramplinLength, 10),
        //        new Vector3(-tramplinWidth/2, 0, 0),

        //        new Vector3(-tramplinWidth/2, -1, 0),
        //        new Vector3(tramplinWidth/2, -1, 0),
        //        new Vector3(tramplinWidth/2, tramplinLength, 10),
        //        new Vector3(-tramplinWidth/2, tramplinLength, 10),
        //        new Vector3(-tramplinWidth/2, -1, 0),
        //    };
        //===============================


        public Field(Game game, Space space, GraphicsDevice graphicsDevice, float scale)
        {
            this.game = game;
            this.space = space;
            this.graphicsDevice = graphicsDevice;
            init();
            game.Reloading += (s, e) => this.Reload();
            this.SetPosition(0.0f, 0.05f, 0.0f);
            setScaling(scale);
            worldMatrix = Fusion.Mathematics.Matrix.Identity * Fusion.Mathematics.Matrix.AffineTransformation(scale, Fusion.Mathematics.Quaternion.Zero, getPosition());
        }

        private float angle = radiousCylinder/10;
        private void init()
        {
            base.LoadContent(game, graphicsDevice, "scenes/cube", shaderName, 1);
            downBox =  new Box(new Vector3(0,0,-2.5f), coorX, coorY, coorZ);
            space.Add(downBox);
            //leftBox = createAndAddBox(new Vector3(-coorX / 2, 0, 0), new Quaternion(0, 1, 0, MathHelper.PiOver2));
            //rightBox = createAndAddBox(new Vector3(coorX / 2, 0, 0), new Quaternion(0, 1, 0, MathHelper.PiOver2));
            //forwardBox = createAndAddBox(new Vector3(0, coorY / 2, 0), new Quaternion(1, 0, 0, MathHelper.PiOver2));
            //backBox = createAndAddBox(new Vector3(0, -coorY / 2, 0), new Quaternion(1, 0, 0, MathHelper.PiOver2));
            // =============================
            // TODO: traplins
            objectTrace = new List<Cylinder>();
            for (int i = 0; i < countObjectTrace; i++)
            {
                var position = new Vector3(radiousCylinder * i, 0, radiousCylinder + angle * i);
                var cylinder = new Cylinder(position, heightCylinder, radiousCylinder);
                objectTrace.Add(cylinder);
                space.Add(cylinder);
            }

            //traplin = new ConvexHull(tramplinVector3List);
            //space.Add(traplin);
            //==============================
        }

        internal void Update(GameTime gameTime, DebugRender dr)
        {
            dr.DrawBox(new BoundingBox(
                            new Fusion.Mathematics.Vector3(downBox.Position.X - coorX / 2, downBox.Position.Z - coorZ / 2,downBox.Position.Y - coorY / 2),
                            new Fusion.Mathematics.Vector3(downBox.Position.X + coorX / 2, downBox.Position.Z + coorZ / 2, downBox.Position.Y + coorY / 2)
                            ),
                        Color.BlueViolet);
            //======================
            // TODO: tramplin from cylinder
            int count = 0;
            foreach (Cylinder cylin in objectTrace)
            {

                dr.DrawBox(new BoundingBox(
                            new Fusion.Mathematics.Vector3(cylin.Position.X - radiousCylinder / 2, radiousCylinder + angle * count, cylin.Position.Y / 2 - heightCylinder / 2),
                            new Fusion.Mathematics.Vector3(cylin.Position.X + radiousCylinder / 2, radiousCylinder + angle * (count + 1), cylin.Position.Y / 2 + heightCylinder / 2)
                            ),
                        Color.BlueViolet);
                count++;
            }
            //===============================
            // TODO: tramplin
            //dr.DrawBox(new BoundingBox(
            //    new Fusion.Mathematics.Vector3(-tramplinWidth / 2, 0, -tramplinLength / 2),
            //    new Fusion.Mathematics.Vector3(tramplinWidth / 2, tramplinHeight, tramplinLength / 2)),
            //    Fusion.Mathematics.Matrix.AffineTransformation(
            //        1,
            //        new Fusion.Mathematics.Quaternion(
            //            traplin.Orientation.X,
            //            traplin.Orientation.Y,
            //            traplin.Orientation.Z,
            //            traplin.Orientation.W),
            //       Fusion.Mathematics.Vector3.Zero),
            //    Color.Cyan);
            //foreach (var pos in tramplinVector3List)
            //{
            //    dr.DrawPoint(switchVectorFromBepu(pos),1, Color.Red);
            //}
            //==============================
        }

        private Box createAndAddBox(Vector3 position, Quaternion quaternion)
        {
            var motionState = new MotionState();
            motionState.Position = position;
            motionState.Orientation = quaternion;
            Box box = new Box(motionState, coorX, coorY, coorZ);
            space.Add(box);
            return box;
        }

        
    }
}
