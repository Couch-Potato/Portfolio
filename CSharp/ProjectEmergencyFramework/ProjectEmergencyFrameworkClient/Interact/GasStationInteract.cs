using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("gasStation", "E", "FUEL VEHICLE", true, GenericInteractAttachment.Gas)]
    public class GasStationInteract : LookatRadiusInteractable
    {
        public GasStationInteract()
        {
            Radius = 1.5f;
            Offset = new CitizenFX.Core.Vector3(0, 0, .7f);
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("gasStation", new { station= DoorService.GasPumps[Entity] });
        }
        protected new async Task<bool> CanShow()
        {
            return false;
        }
    }
}
