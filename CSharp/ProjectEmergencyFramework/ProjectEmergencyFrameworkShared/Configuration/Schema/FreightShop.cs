using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class FreightShop
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public string Archetype { get; set; }
        public string Skin { get; set; } // biligeco or jetsam
    }
}
