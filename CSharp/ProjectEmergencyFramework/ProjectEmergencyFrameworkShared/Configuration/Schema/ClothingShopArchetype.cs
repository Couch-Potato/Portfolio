using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class ClothingShopArchetype : IArchetype
    {
        public string Name { get; set; }
        public List<ClothingShopItem> Items { get; set; } = new List<ClothingShopItem>();
    }
}
