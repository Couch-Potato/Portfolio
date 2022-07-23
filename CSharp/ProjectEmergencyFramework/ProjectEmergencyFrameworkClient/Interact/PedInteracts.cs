using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("ex_0", "E", "Give Items", true)]
    public class ExchangeItemsInteract : LookatRadiusInteractable
    { 
        public ExchangeItemsInteract()
        {
            Radius = 3f;
            Tolerance = 30f / 2;
            RequireControlKeyDown = true;
        }
        protected override void OnInteract()
        {
            // First lets create a custom container.
            var invx = new List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem>();

            for (int i = 0; i < 15; i++)
            {
                invx.Add(new ProjectEmergencyFrameworkShared.Data.Model.InventoryItem());
            }

            var c = new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                Name = "Give Items",
                Id = "ex_0",
                MaxItems = 15,
                Type = "CUSTOM",
                Inventory = invx
            };

            Services.InventoryService.OpenCustomInventory(c, (List<ProjectEmergencyFrameworkShared.Data.Model.InventoryItem> items) =>
            {

            });
        }
    }
}
