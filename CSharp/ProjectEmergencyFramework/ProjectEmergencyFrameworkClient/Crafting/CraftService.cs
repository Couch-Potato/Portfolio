using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Crafting
{
    public static class CraftService
    {
        public static void ShowCraftInterface(CraftInterface craftInterface)
        {
            InventoryService.ShowingInventory = true;
            Interfaces.InterfaceController.ShowInterface("inventory", new { 
                craftConfigure = new CraftingConfiguration()
                {
                    CraftInterfaceType = craftInterface,
                    CraftPressed = async (InventoryItemCollection inv, Action clearAndRefresh) =>
                    {
                        foreach (var recipe in CraftingRecipes[craftInterface])
                        {
                            if (await recipe.Validate(inv))
                            {
                                var doClear = await recipe.Craft(inv);
                                if (doClear) clearAndRefresh();
                            }
                        }
                    }
                }
            });
        }

        private static Dictionary<CraftInterface, List<IRecipe>> CraftingRecipes = new Dictionary<CraftInterface, List<IRecipe>>();

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void DiscoverRecipes()
        {
            foreach (CraftInterface intfs in (CraftInterface[])Enum.GetValues(typeof(CraftInterface)))
            {
                CraftingRecipes.Add(intfs, new List<IRecipe>());
            }

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!t.IsClass) continue;
                if (!(t.GetCustomAttribute<CraftingRecipeAttribute>() != null)) continue;

                var attb = t.GetCustomAttribute<CraftingRecipeAttribute>();
                DebugService.DebugCall("CRAFTING", "Importing crafting recipe: " + attb.Name);
                
                try
                {
                    CraftingRecipes[attb.CraftInterface].Add((IRecipe)Activator.CreateInstance(t));
                }catch(Exception ex)
                {
                    DebugService.UnhandledException(ex);
                    DebugService.DebugWarning("CRAFTING", $"Error importing crafting recipe {attb.Name}: {ex.Message}");
                    
                }

            }
        }
    }
}
