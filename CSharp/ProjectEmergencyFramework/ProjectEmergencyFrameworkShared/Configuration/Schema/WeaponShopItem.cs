using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class WeaponShopItem
    {
        public string Name { get; set; }
        public string IconId { get; set; }
        public float Price { get; set; }
        public uint WeaponHash { get; set; }
    }
}
