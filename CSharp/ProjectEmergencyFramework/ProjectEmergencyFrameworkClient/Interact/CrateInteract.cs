using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("crateShop", "E", "ENTER CRATE SHOP")]
    class CrateShopInteract : RadiusInteractable
    {
        public CrateShopInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("crateShop", Properties);
        }
    }
}
