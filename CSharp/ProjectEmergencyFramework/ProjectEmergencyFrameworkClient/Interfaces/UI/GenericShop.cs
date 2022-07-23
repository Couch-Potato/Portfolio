using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("genericShop", true)]
    public class GenericShop : UserInterface
    {
        private List<BasedShopItem> _items;

        [Configuration("shopData")]
        public List<BasedShopItem> shopItems { get=>_items; set=>_items = value; }

        private int _p = -1;

        [Reactive("purchase")]
        public int purchase { get => _p; set => handleGive(value); }
        protected override Task ConfigureAsync()
        {
            shopItems = Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<List<BasedShopItem>>(Properties.items);
            return base.ConfigureAsync();
        }

        [Reactive("_hide")]
        public void exit2()
        {
            Hide();
        }
        private async void handleGive(int id)
        {
            if (!await TransactionService.Pay(_items[id].price, "SHOP_PURCHASE - " + _items[id].name.ToUpper()))
                return;
            _p = id;
            if (_items[id].smartItemName != null)
            {
                if (_items[id].smartItemName != "NULL")
                {
                    InventoryService.AddItem(EquipmentService.ConstructEquipable(_items[id].smartItemName, "", _items[id].modifiers));
                }

            }
            Services.InventoryService.AddItem(_items[id].name, _items[id].icon, _items[id].modifiers);

        }
    }
    public class BasedShopItem
    {
        public string icon { get; set; }
        public string name { get; set; }
        public float price { get; set; }
        public dynamic modifiers { get; set; }
        public string type { get; set; }
        public string smartItemName { get; set; }
    }
}
