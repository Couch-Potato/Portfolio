using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("crafting", "G", "CRAFTING", true)]
    public class CraftingTableInteract : RadiusInteractable
    {
        public CraftingTableInteract()
        {
            Radius = 10f;
        }
        protected override void OnInteract()
        {
            
            InventoryService.AddItem(EquipmentService.ConstructEquipable("Meth", "", new {}));
            Crafting.CraftService.ShowCraftInterface(Properties.interfaceType);
        }
    }
}
