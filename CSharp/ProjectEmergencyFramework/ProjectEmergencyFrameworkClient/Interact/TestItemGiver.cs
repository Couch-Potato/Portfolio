using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("testitem", "E", "GET ITEM")]
    public class TestItemGiver : RadiusInteractable
    {
        public TestItemGiver()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            
            InventoryService.AddItem(EquipmentService.ConstructEquipable("Briefcase", "", new {}));
        }
    }
}
