using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("weaponShop", true)]
    public class WeaponShop : UserInterface
    {
        private int i_ = -1;
        private List<WeaponItem> weaponItems = new List<WeaponItem>();

        [Configuration("weapons")]
        public List<WeaponItem> Weapons { get =>weaponItems; }
        
        [Reactive("purchase")]
        public int Purchase { get=>i_; set=>HandlePurchase(value); }

        bool wasPurchaseEffectuated = false;
        private async void HandlePurchase(int id)
        {
            if (wasPurchaseEffectuated) return;
            wasPurchaseEffectuated = true;
            if (!await TransactionService.Pay(weaponItems[id].price, "WEAPON_PURCHASE - " + weaponItems[id].name.ToUpper()))
                return;

            dynamic props = new ExpandoObject();
            props.name = weaponItems[id].name;
            props.icon = weaponItems[id].icon;
            props.weapon_hash = weaponItems[id].WeaponHash;
            InventoryService.AddItem(EquipmentService.ConstructEquipable("GUN", "__gun", props));
            Hide();
        }

        [Reactive("_hide")]
        public void Hide2()
        {
            Hide();
        }

        protected override Task ConfigureAsync()
        {
            weaponItems = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<List<WeaponItem>>(Properties.items);
            return base.ConfigureAsync();
        }

    }
    public class WeaponItem
    {
        public string icon { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public uint WeaponHash { get; set; }
    }
}
