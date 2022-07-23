using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("genericShop", "E", "ENTER SHOP")]
    public class GenericShopInteract:RadiusInteractable
    {
        public GenericShopInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("genericShop", Properties);
        }
    }
}
