using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("vehiclespawn", "E", "SPAWN VEHICLE")]
    public class VehicleSpawnInteract : RadiusInteractable
    {
        public VehicleSpawnInteract()
        {
            Radius = 15f;
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("vehicleselector", Properties);
        }
        protected new async Task<bool> CanShow()
        {
            return await base.CanShow() && true;
        }
    }
}
