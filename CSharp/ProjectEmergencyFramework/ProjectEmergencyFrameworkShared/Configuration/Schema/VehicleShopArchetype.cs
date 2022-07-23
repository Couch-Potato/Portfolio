using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class VehicleShopArchetype : IArchetype
    {
        public string Name { get; set; }
        public List<VehicleShopItem> Items { get; set; } = new List<VehicleShopItem>();
    }
}
