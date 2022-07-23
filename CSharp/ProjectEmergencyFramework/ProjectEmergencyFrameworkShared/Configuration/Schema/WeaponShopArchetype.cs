using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class WeaponShopArchetype : IArchetype
    {
        public string Name { get; set; }
        public List<WeaponShopItem> Items { get; set; } = new List<WeaponShopItem>();
    }
}
