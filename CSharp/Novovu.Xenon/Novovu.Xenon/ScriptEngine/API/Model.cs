using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class Model
    {
        public BasicModel basicModel_reference;
        public Mesh[] Meshes;
        public Model(BasicModel reff)
        {
            basicModel_reference = reff;
            Meshes = new Mesh[reff.ObjectMeshes.Length];
            for (int i = 0; i < Meshes.Length; i++)
            {
                Meshes[i] = new Mesh(reff.ObjectMeshes[i]);
            }
        }
    }
}