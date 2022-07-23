using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SVector3
    {
        public float X;
        public float Y;
        public float Z;

        public static SVector3 From(Vector3 vector3)
        {
            return new SVector3
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
        }
        public Vector3 Import()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
