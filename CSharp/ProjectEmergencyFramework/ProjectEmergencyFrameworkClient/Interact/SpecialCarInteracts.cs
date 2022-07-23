using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("specialTrunk", "E", "Open Trunk", true)]
    public class SpecialCarInteract : RadiusInteractable
    {
        public SpecialCarInteract()
        {
            Radius = 10f;
            Offset = CitizenFX.Core.Vector3.Down * 1.7f;
            RequireControlKeyDown = true;
        }
        protected override void OnInteract()
        {
     
            InventoryService.OpenCustomInventory(new Container() { MaxItems= VehicleService.SpecialTrunkItems.Count, Name="TRUNK", Type="TRUNK", Inventory=VehicleService.SpecialTrunkItems}, (List<InventoryItem> items) =>
            {
                VehicleService.SpecialTrunkItems.Clear();
                VehicleService.SpecialTrunkItems.AddRange(items);
            });
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow() && true;
        }
    }
}
