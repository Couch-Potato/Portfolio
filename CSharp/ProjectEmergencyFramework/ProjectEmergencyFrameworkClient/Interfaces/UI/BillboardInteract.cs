using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("interactBillboard", false)]
    public class BillboardInteract : UserInterface
    {
        private List<BillboardInteractItem> _items = new List<BillboardInteractItem>();
        [Configuration("interacts")]
        public List<BillboardInteractItem> items { get => _items; set {
                _items = value;
                Update();
            } }

        public void AddInteract(BillboardInteractItem item)
        {
            var svx = _items;
            svx.Add(item);
            items = svx;
        }
        public void RemoveInteract(BillboardInteractItem item)
        {
            var svx = _items;
            svx.Remove(item);
            items = svx;
        }
        public void UpdateInteractLocation(string key, string caption, BillboardLocation oldLocation, BillboardLocation newLocation)
        {
            foreach (var itc in _items)
            {
                if (itc.keyBind == key && itc.caption == caption && itc.location.x == oldLocation.x && itc.location.y == oldLocation.y)
                {
                    itc.location = newLocation;
                    Update();
                    return;
                }
            }
        }
        public void UpdateInteractActiveStatus(string key, string caption, BillboardLocation oldLocation, bool active, bool update = false)
        {
            foreach (var itc in _items)
            {
                if (itc.keyBind == key && itc.caption == caption && itc.location.x == oldLocation.x && itc.location.y == oldLocation.y)
                {
                    itc.active = active;
                    if (update)
                        Update();
                    return;
                }
            }
        }
    }

    public class BillboardInteractItem
    {
        public string keyBind { get; set; }
        public string caption { get; set; }
        public bool active { get; set; }
        public bool locked { get; set; }

        public BillboardLocation location { get; set; }
        
    }
    public class BillboardLocation
    {
        public float x { get; set; }
        public float y { get; set; }
    }
}
