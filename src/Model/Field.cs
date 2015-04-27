using System;
using System.Collections.Generic;
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
        private float coorX=1000;
        private float coorY=1000;
        private float coorZ=0f;
        private Box box;
        private Box traplin;

        private float heightCylinder=30;
        private static float radiousCylinder=0.25f;
        private const int countObjectTrace=120;
        private const float distance = 30;
        private List<Cylinder> objectTrace;

        private float x = 0;
        private float y = 0;
        private float z = 1;
        private float w = 1;

        //===============================
        private float tramplinLength = 100;
        private float tramplinWidth = 100;
        private float tramplinHeight = 0; 
        //===============================


        public Field(Game game, Space space, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.space = space;
            this.graphicsDevice = graphicsDevice;
            init();
        }

        private float angle = radiousCylinder/10;
        private void init()
        {
            //===============================
            var ms = new MotionState();
            ms.Position = Vector3.Zero;
            ms.Orientation = new Quaternion(0,1,0,0);
            traplin = new Box(ms, tramplinWidth, tramplinHeight, tramplinLength);
            space.Add(traplin);
            //==============================
            box = new Box(Vector3.Zero, coorX, coorY, coorZ);
            space.Add(box);
            objectTrace = new List<Cylinder>();
            for (int i = 0; i < countObjectTrace; i++)
            {
                var position = new Vector3(radiousCylinder * i, 0, radiousCylinder + angle * i);
                var cylinder = new Cylinder(position, heightCylinder, radiousCylinder);
                objectTrace.Add(cylinder);
                space.Add(cylinder);
            }
        }

        internal void Update(GameTime gameTime, DebugRender dr)
        {
            //dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(-coorX / 2, 0, -coorY / 2), new Fusion.Mathematics.Vector3(coorX / 2, coorZ, coorY / 2)), switchMatrixFromBepu(box.WorldTransform) ,Color.Cyan);
            dr.DrawGrid(100);
            int count = 0;
            foreach (Cylinder cylin in objectTrace)
            {
                dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(cylin.Position.X - radiousCylinder / 2, radiousCylinder+angle*count, cylin.Position.Y / 2 - heightCylinder / 2),
                            new Fusion.Mathematics.Vector3(cylin.Position.X + radiousCylinder / 2, radiousCylinder+angle*(count+1), cylin.Position.Y / 2 + heightCylinder / 2)), Color.Red);
                count++;
                //dr.DrawBox(
                //    new BoundingBox(new Fusion.Mathematics.Vector3(cylin.Position.X - radiousCylinder / 2, 0, cylin.Position.Y / 2 - heightCylinder / 2),
                //        new Fusion.Mathematics.Vector3(cylin.Position.X + radiousCylinder / 2, radiousCylinder, cylin.Position.Y / 2 + heightCylinder / 2)),
                //        Fusion.Mathematics.Matrix.RotationY((float)Math.PI / 2),
                //        Color.Cyan);
            }

            //===============================
            dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(-tramplinWidth / 2, 0, -tramplinLength / 2), new Fusion.Mathematics.Vector3(tramplinWidth / 2, tramplinHeight, tramplinLength / 2)), Fusion.Mathematics.Matrix.AffineTransformation(1, new Fusion.Mathematics.Quaternion(traplin.Orientation.X, traplin.Orientation.Y,traplin.Orientation.Z,traplin.Orientation.W), Fusion.Mathematics.Vector3.Zero), Color.Cyan);
            //==============================
        }
    }
}
