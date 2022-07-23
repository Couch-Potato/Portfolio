using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class GenericShopItem
    {
        public string Name { get; set; }
        public string GenericItemName { get; set; }
        public float Price { get; set; }
        public string Category { get; set; }
    }
}
