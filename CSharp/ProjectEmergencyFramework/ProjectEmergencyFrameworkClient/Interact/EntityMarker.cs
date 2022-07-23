using System;
using System.Collections.Generic;
using System.Linq;
using static CitizenFX.Core.Native.API;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public class EntityMarker
    {
        public int Handle { get; }
        public bool IsCar { get; set; }
        public EntityMarker(int hand, bool isCar = false)
        {
            Handle = hand;
            IsCar = isCar;
        }

        public void Tick()
        {
            var coords = GetEntityCoords(Handle, true);
            DrawMarker(IsCar ? 1 : 23, coords.X, coords.Y, coords.Z, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, IsCar ? 5f : 2f, IsCar ? 5f : 2f, IsCar ? 5f : 2f, 0, 48, 73, 128, false, true, 2, false, null, null, true);
        }
    }
}
