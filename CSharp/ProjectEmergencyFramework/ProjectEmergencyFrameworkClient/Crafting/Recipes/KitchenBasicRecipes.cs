using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Crafting.Recipes
{
    [CraftingRecipe("cr_food@government_cheese", Interfaces.UI.CraftInterface.FoodCrafter)]
    public class GovernmentCheese : IRecipe
    {
        public async Task<bool> Craft(InventoryItemCollection items)
        {
            if (await Validate(items))
            {
                InventoryService.AddItem("Government Cheese", "", 1, false, new
                {
                    desc="Because god bless the US government!",
                    tags="CHEESE,GOVERNMENT"
                });
                return true;
            }
            return false;
        }

        public async Task<bool> Validate(InventoryItemCollection items)
        {
            return items.HasItem("Meth", "");
        }
    }
}
