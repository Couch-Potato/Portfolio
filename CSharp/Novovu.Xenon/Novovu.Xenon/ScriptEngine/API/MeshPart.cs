using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class MeshPart
    {
        VertexPositionNormalTexture[] reffx;
        VertexPositionNormalTexture[] Verticies { get { return reffx; }set { ReferencePart.VertexBuffer.SetData(value); reffx = value; } }
        Microsoft.Xna.Framework.Graphics.ModelMeshPart ReferencePart;
        public MeshPart(Microsoft.Xna.Framework.Graphics.ModelMeshPart reff)
        {
            ReferencePart = reff;
         //   ReferencePart.VertexBuffer.GetData(reffx);
        }

    }
}
