using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Lighting
{
    public class PointLight : IObjectLight
    {
        private Vector3 _poffset = new Vector3(0, 0, 0);
        public Vector3 PositionOffset { get {
                return _poffset;
            } set {
                _poffset = value;
            } }
        public float Distance { get; set; }
        public float FalloffExponet { get; set; }
        public float MaxDistance { get; set; }
        public Engine.GameObject Parent { get; set; }
        public Vector3 Color { get; set; }
    }
}