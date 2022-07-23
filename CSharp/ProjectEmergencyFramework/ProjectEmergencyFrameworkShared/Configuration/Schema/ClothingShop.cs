using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class ClothingShop
    {
        public string Archetype { get; set; }
        public Location Location { get; set; } = new Location();
        public string Name { get; set; }
        public string Memo { get; set; }
        public BlipInfo BlipInfo { get; set; }

    }
}
