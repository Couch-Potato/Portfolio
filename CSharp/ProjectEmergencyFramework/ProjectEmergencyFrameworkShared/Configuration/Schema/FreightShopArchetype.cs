using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class FreightShopArchetype : IArchetype
    {
        public string Name { get; set; }
        public List<FreightShopItem> ItemsForSale { get; set; }
        public List<FreightShopItem> ItemsBuying { get; set; }

        public List<CraftingModuleShopConfig> CraftingModulesForSale { get; set; }
    }
    public class FreightShopItem
    {
/*        public string Name { get; set; }*/
        public string Description { get; set; }
        public string GenericItemName { get; set; }
        public float Price { get; set; }

    }
    public class CraftingModuleShopConfig
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public string Icon { get; set; }
        public string ModuleType { get; set; }
        public string Description { get; set; }
        public int Tier { get; set; }
    }
    
}
