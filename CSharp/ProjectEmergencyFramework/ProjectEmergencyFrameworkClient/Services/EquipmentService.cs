using ProjectEmergencyFrameworkClient.Equipables;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class EquipmentService
    {
        public static List<DiscoveredItem> Equipables;
        [ExecuteAt(ExecutionStage.Initialized)]
        public static void RegisterEquipment()
        {
            Equipables = ClassDiscovery.DiscoverWithAttribute<EquipableAttribute>();
        }
        public static Equipable ConstructEquipable(string name, string icon, dynamic mods, bool discrete = false)
        {
            // DebugService.DebugCall("EQUIP", "CONSTRUCT: " + name); 
           
            foreach (var item in Equipables)
            {
                var attb = item.GetAttribute<EquipableAttribute>();
                if (attb.Name == name)
                {
                    var eqp = item.ConstructAs<Equipable>();
                    var mods2 = Utility.CrappyWorkarounds.JSONDynamicToExpando(mods); 
                    mods2.O_NAME = name;
                    mods2.O_ICON = icon;
                    if (!CrappyWorkarounds.HasProperty(mods2, "DynamicInstanceId"))
                    {
                        //DebugService.Watchpoint("EQUIPCONSTRUCT", mods2);
                        var dynInst = RandomUtil.RandomString(16);
                        mods2.DynamicInstanceId = dynInst;
                    }
                    
                    
                    eqp.CreateInstance(mods2, discrete);
                    return eqp;
                }
            }
            return null;
        }
        public static Equipable ReconstructEquipable(Equipable eqp, dynamic mods)
        {
            var mods2 = Utility.CrappyWorkarounds.JSONDynamicToExpando(mods);
            eqp.CreateInstance(mods2, false);
            return eqp;

        }
    }
}
