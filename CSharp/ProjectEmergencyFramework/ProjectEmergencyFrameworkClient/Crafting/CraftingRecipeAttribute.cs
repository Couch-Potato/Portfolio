using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Crafting
{
    public class CraftingRecipeAttribute : Attribute
    {
        public string Name { get; set; }
        public Interfaces.UI.CraftInterface CraftInterface { get; set; }
        public CraftingRecipeAttribute(string name, Interfaces.UI.CraftInterface ci) { 
            Name = name;
            CraftInterface = ci;
        }

    }
}
