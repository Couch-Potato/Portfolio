using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Lighting
{
    public interface IObjectLight
    {
        float Distance { get; set; }
        float FalloffExponet { get; set; }
        float MaxDistance { get; set; }

        Engine.GameObject Parent { get; set; }
        Vector3 PositionOffset { get; set; }
        Vector3 Color { get; set; }
    }
}