using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fusion;
using Fusion.Graphics;
using Fusion.Input;
using Fusion.Content;
using System.Runtime.InteropServices;

namespace MIGProject
{
        struct ModelConstData
        {
            public Matrix Projection;
            public Matrix View;
            public Matrix World;
            public Vector4 ViewPos;
            /*
            public Vector4	SkyLightDir;
            public Vector4	SkyLightColor;
            public Vector4	LightPos0;
            public Vector4	LightPos1;
            public Vector4	LightPos2;
            public Vector4	LightColor0;
            public Vector4	LightColor1;
            public Vector4	LightColor2;*/
    }
}
