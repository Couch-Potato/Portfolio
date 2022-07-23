using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("multiInteract")]
    public class MultiInteract : UserInterface
    {
        private List<MultiInteractItem> _items = new List<MultiInteractItem>();
        [Configuration("interacts")]
        public List<MultiInteractItem> items
        {
            get => _items; set
            {
                _items = value;
                Update();
            }
        }

        private int _selected = 0;

        [Configuration("selected")]
        public int Selected { get => _selected; set {
                _selected = value;
                Update();
            } 
        }

    }
    public class MultiInteractItem
    {
        public string caption { get; set; }
        public bool locked { get; set; }


    }
}
