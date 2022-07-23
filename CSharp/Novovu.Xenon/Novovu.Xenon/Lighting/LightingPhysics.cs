using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Lighting
{
    public class LightingPhysics
    {
        public static float GetKatt(BasicModel objs, IObjectLight light)
        {
            float a = light.MaxDistance;
            float d = Engine.Engine.GetDistanceOfPoints(objs.LocationOffset + objs.ParentObject.Location, light.PositionOffset + light.Parent.Location);
            float f = light.FalloffExponet;
            return 1 - (d / a) * f;
        }
        public static float GetKatt(Animation.AnimatedModel objs, IObjectLight light)
        {
            float a = light.MaxDistance;
            float d = Engine.Engine.GetDistanceOfPoints(objs.LocationOffset + objs.ParentObject.Location, light.PositionOffset + light.Parent.Location);
            float f = light.FalloffExponet;
            return 1 - (d / a) * f;
        }
    }
}