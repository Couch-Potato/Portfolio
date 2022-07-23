using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Crafting
{
    public interface IRecipe
    {
        Task<bool> Craft(InventoryItemCollection items);
        Task<bool> Validate(InventoryItemCollection items);
    }
}
