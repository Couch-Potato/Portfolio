using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class InventoryItem
    {
        public string name { get; set; }
        public string icon { get; set; }
        public int qty { get; set; }
        public bool isStackable {get;set;}
        public dynamic modifiers { get; set; }
        public string transportString { get; set; }
    }
}
