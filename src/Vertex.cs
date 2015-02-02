using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Graphics;
using Fusion.Input;
using System.Runtime.InteropServices;


namespace MIGProject
{
     public struct Vertex
        {
            [Vertex("POSITION")]
            public Vector3 Position;
            [Vertex("TEXCOORD", 0)]
            public Vector2 TexCoord;
            [Vertex("COLOR")]
            public Color Color;
        }
}
