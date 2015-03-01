using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BEPUphysics;
using BEPUphysics.Entities.Prefabs;
using Fusion;
using Fusion.Graphics;
using Fusion.Mathematics;
using Vector3 = BEPUutilities.Vector3;

namespace ExampleFlight.src.Model
{
    class Field : ModelOfScene
    {
        private Space space;
        private float coorX=100;
        private float coorY=100;
        private float coorZ=0f;
        private Box box;

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
            
        }

        internal void Update(GameTime gameTime, DebugRender dr)
        {
            dr.DrawBox(new BoundingBox(new Fusion.Mathematics.Vector3(-coorX/2, 0, -coorY/2), new Fusion.Mathematics.Vector3(coorX/2, coorZ, coorY/2)), Color.Cyan);
        }
    }
}
