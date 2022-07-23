using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.RPC.RPCFunctions
{
    public static class Inventory
    {
        [RPCFunction("InventoryUpdate")]
        public static void InventoryUpdate(object a)
        {
            Task.Run(async () =>
            {
                await Services.InventoryService.ConfigureInventory();
            });
        }
        [RPCFunction("InventoryAdd")]
        public static void InventoryAdd(object a)
        {
            var lx = (List<InventoryItem>)a;
            foreach (var item in lx)
            {
            //    Services.InventoryService.AddItem()
            }
        }
        [RPCFunction("GIVE_ITEM")]
        public static void GiveItem(object a)
        {
            InventoryItem item = (InventoryItem)a;
            if (item.transportString != null)
            {
                item.modifiers = new ExpandoObject();
                item.modifiers = JsonConvert.DeserializeAnonymousType(item.transportString, item.modifiers);
            }
            if (item.modifiers != null)
            {
                if (CrappyWorkarounds.HasProperty(item.modifiers, "O_NAME"))
                {
                    var ix = EquipmentService.ConstructEquipable(item.modifiers.O_NAME, item.modifiers.O_ICON, item.modifiers);
                    InventoryService.AddItem(ix);
                }
            }
        }
        [RPCFunction("ADD_CASH_PLAYER")]
        public static void GiveCash(object a)
        {
            float amt = (float)a;
            MoneyService.Cash += amt;
            QueryService.QueryConcrete<bool>("ADD_CASH", amt);
        }
    }
}
