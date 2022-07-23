using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("inv_notify")]
    public class InventoryNotification : UserInterface
    {
        private List<InventoryNotificationItem> _items = new List<InventoryNotificationItem>();
        public InventoryNotification()
        {
        }
        [Configuration("items")]
        public List<InventoryNotificationItem> Items { get => _items; }

        public void Notify(string icon, string name, int qty)
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var item = new InventoryNotificationItem()
            {
                icon = icon,
                name = name,
                qty = qty,
                startTime = unixTimestamp
            };
            Items.Add(item);
            HandleNotifyExpire(item);
            Update();
        }

        private async void HandleNotifyExpire(InventoryNotificationItem item)
        {
            await BaseScript.Delay(10000);
            Items.Remove(item);
            Update();
        }

    }
    public class InventoryNotificationItem
    {
        public string icon { get; set; }
        public string name { get; set; }
        public int qty { get; set; } 
        public int startTime { get; set; }
    }
}
