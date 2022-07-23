using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("weaponShop", "E", "ENTER SHOP")]
    public class WeaponShopInteract : RadiusInteractable
    {
        public WeaponShopInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface("weaponShop", Properties);
        }
    }
    [Interactable("clothingShop", "E", "ENTER SHOP")]
    public class ClothingShopInteract : RadiusInteractable
    {
        public ClothingShopInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            DebugService.Watchpoint("BEFORESHOW", null);

            Interfaces.InterfaceController.ShowInterface("clothingshop", Properties);
        }
    }
}
