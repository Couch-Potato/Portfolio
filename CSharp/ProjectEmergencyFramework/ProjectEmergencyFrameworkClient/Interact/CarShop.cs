using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("carshop", "E", "ENTER CAR SHOP")]
    class CarShopInteract : RadiusInteractable
    {
        public CarShopInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("carshop", Properties);
        }
    }
}
