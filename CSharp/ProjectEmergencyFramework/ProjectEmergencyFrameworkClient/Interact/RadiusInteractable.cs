using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Interact
{
    public abstract class RadiusInteractable : Interactable
    {
        public float Radius { get; protected set; }
        public override async Task<bool> CanShow()
        {
            var baseCanShow = await base.CanShow();
            var coords = GetEntityCoords(Game.PlayerPed.Handle, true);
            float dist = Vector3.Distance(coords, Position);
            return dist < Radius && baseCanShow;
        }

        protected abstract override void OnInteract();
    }
}
