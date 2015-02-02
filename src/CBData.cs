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

namespace ExampleFlight
{
    struct CBData
    {
        public Matrix Projection;
        public Matrix View;
        public Matrix World;
        public Vector4 ViewPos;
    }
}
