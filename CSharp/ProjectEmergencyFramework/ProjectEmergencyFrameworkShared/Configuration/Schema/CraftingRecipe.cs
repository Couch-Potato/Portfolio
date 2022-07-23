using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class CraftingRecipe
    {
        public string RecipeType { get; set; }
        public string ItemResult { get; set; }
        public List<RecipeItem> RequiredItems { get; set; }
        public string RecipeName { get; set; }
    }
    public class RecipeItem
    {
        public string Name { get; set; }
        public string IconId { get; set; }
    }
}
