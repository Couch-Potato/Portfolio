using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class GenericShopArchetype : IArchetype
    {
        public string Name { get; set; }
        public List<GenericShopItem> Items { get; set; }
    }
}
