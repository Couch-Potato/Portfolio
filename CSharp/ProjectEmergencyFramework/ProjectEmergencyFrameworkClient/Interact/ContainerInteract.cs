using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("container", "E", "Open Trunk", true)]
    public class ContainerInteract : RadiusInteractable
    {
        public ContainerInteract()
        {
            Radius = 10f;
            Offset = CitizenFX.Core.Vector3.Down * 1.7f;
            RequireControlKeyDown = true;
        }
        protected override void OnInteract()
        {
            DebugService.Watchpoint("CONTAINER OPEN", null);
            InventoryService.ShowingInventory = true;
            Interfaces.InterfaceController.ShowInterface("inventory", Properties);
        }
        protected new async Task<bool> CanShow()
        {
            
            return await base.CanShow() && true;
        }
    }
}
