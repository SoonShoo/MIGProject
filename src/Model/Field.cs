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


        private float heightCylinder=1000;
        private float radiousCylinder=0.15f;
        private const int countObjectTrace=1;
        private const float distance = 30;
        private List<Cylinder> objectTrace;

        private float x = 0;
        private float y = 0;
        private float z = 1;
        private float w = 1;




        public Field(Game game, Space space, GraphicsDevice graphicsDevice)
        {
            this.game = game;
            this.space = space;
            this.graphicsDevice = graphicsDevice;
            init();
        }

        private void init()
        {           
            box = new Box(Vector3.Zero, coorX, coorY, coorZ);
            space.Add(box);
            objectTrace = new List<Cylinder>();
            //for (int i = 0; i < countObjectTrace; i++)
            //{
            //    var ms = new MotionState();
            //    ms.Position = new Vector3(0, 0, 0);
            //    //ms.Orientation = BEPUutilities.Quaternion.CreateFromAxisAngle(new Vector3(x, y, z), w);
            //    var cylinder = new Cylinder(new Vector3(0, 0, 0), heightCylinder, radiousCylinder);
            //    objectTrace.Add(cylinder);
            //    space.Add(cylinder);
            //}
        }

        internal void Update(GameTime gameTime, DebugRender dr)
        {
            dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(-coorX/2, 0, -coorY/2), new Fusion.Mathematics.Vector3(coorX/2, coorZ, coorY/2)), Color.Cyan);
            dr.DrawGrid(100);
            
            foreach (Cylinder cylin in objectTrace)
            {
                dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(cylin.Position.X - radiousCylinder / 2, 0, cylin.Position.Y / 2 - heightCylinder / 2),
                            new Fusion.Mathematics.Vector3(cylin.Position.X + radiousCylinder / 2, radiousCylinder, cylin.Position.Y / 2 + heightCylinder / 2)), switchMatrixFromBepu(cylin.WorldTransform), Color.Red);
                //dr.DrawBox(
                //    new BoundingBox(new Fusion.Mathematics.Vector3(cylin.Position.X - radiousCylinder / 2, 0, cylin.Position.Y / 2 - heightCylinder / 2),
                //        new Fusion.Mathematics.Vector3(cylin.Position.X + radiousCylinder / 2, radiousCylinder, cylin.Position.Y / 2 + heightCylinder / 2)),
                //        Fusion.Mathematics.Matrix.RotationY((float)Math.PI / 2),
                //        Color.Cyan);
            }
        }
    }
}
