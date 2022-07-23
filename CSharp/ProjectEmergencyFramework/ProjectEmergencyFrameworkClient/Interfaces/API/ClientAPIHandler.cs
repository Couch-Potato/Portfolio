using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.API
{
    public delegate object APIMethod(object a);

    public class ClientAPIHandler
    {
        static List<DiscoveredItem> discoveredItems;
        static Dictionary<string, APIMethod> calls;
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void Initialize()
        {
         /*   foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ClientAPIAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ClientAPIAttribute>();
                            var dg = (APIMethod)method.CreateDelegate(typeof(APIMethod));
                            calls.Add(attb.ClientAPIName, dg);

                        }
                    }
                }
            }
*/

            // Register Client Events

            InterfaceController.RegisterNUIEvent("clientAPI", (IDictionary<string, object> info, object data) =>
            {
                try
                {
                    var returnValue = Call((string)info["name"], data);
                    InterfaceController.SendNUI("CAPI_" + (string)info["name"], (string)info["ui"], returnValue);
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }

            });
            InterfaceController.RegisterNUIEvent("serverAPI", async (IDictionary<string, object> info, object data) =>
            {
                try
                {
                    var returnValue = await QueryService.Query<object>((string)info["name"], data); // stfu i know what I am doing.
                    InterfaceController.SendNUI("SAPI_" + (string)info["name"], (string)info["ui"], returnValue);
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
            });
        }

        public static object Call(string apiName, object paramsData)
        {
            return calls[apiName](paramsData);
        }
    }
}
