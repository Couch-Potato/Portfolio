using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("trashcan", "G", "USE TRASHCAN", true)]
    public class TrashcanInteract : RadiusInteractable
    {
        public TrashcanInteract()
        {
            Radius = 2f;
        }
        protected override void OnInteract()
        {
            InventoryItemCollection items = new InventoryItemCollection();
            /*    items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuffs", "", new { }));
                items.AddItem(EquipmentService.ConstructEquipable("Handcuff Key", "", new { }));*/

            for (int i = 0; i < 15; i++)
            {
                items.Add(new ProjectEmergencyFrameworkShared.Data.Model.InventoryItem());
            }

            InventoryService.OpenCustomInventory(new ProjectEmergencyFrameworkShared.Data.Model.Container()
            {
                MaxItems = items.Count,
                Inventory = items,
                Name = "TRASHCAN",
                Type = "TRASHCAN"
            }, (List<InventoryItem> itemsRet) =>
            {
                
            });
        }
    }
}
