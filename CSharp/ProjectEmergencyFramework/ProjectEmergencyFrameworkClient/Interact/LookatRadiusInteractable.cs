using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
using CitizenFX.Core;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public abstract class LookatRadiusInteractable : Interactable
    {
        public float Tolerance { get; protected set; }
        public float Radius { get; protected set; }
        public override async Task<bool> CanShow()
        {
            if (RequireControlKeyDown)
            {
                if (!IsControlPressed(0, 19) && !IsDisabledControlPressed(0, 19)) return false;
            }
            // Dont want to waste the time if we are not even in range. 

            var coords = GetEntityCoords(Game.PlayerPed.Handle, true);
            float dist = Vector3.Distance(coords, Position);
            return dist < Radius;
          /*  var coords = GetEntityCoords(Game.PlayerPed.Handle, true);
            var lookat = Math.DegMath.Atan2(Position.Z - coords.Z, Position.X - coords.X);
            if (lookat < 0)
                lookat += 360;
            var entityHeading = GetEntityHeading(Game.PlayerPed.Handle);
            Debug.WriteLine(Tolerance.ToString());
            if (entityHeading - lookat < Tolerance && entityHeading - lookat > Tolerance)
            {
                return true;
            }
            return false;*/
            
        }

        protected abstract override void OnInteract();
    }
}
