using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class Mesh
    {
        private Microsoft.Xna.Framework.Graphics.ModelMesh modelMesh;
        public MeshPart[] Parts;
        public Mesh(ModelMesh reff)
        {
            modelMesh = reff;
            Parts = new MeshPart[reff.MeshParts.Count];
            for (int i=0;i<Parts.Length;i++)
            {
                Parts[i] = new MeshPart(reff.MeshParts[i]);
            }
        }
    }
}
