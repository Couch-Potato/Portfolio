using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using CitizenFX.Core.UI;
using CitizenFX.Core.Native;
using static CitizenFX.Core.Native.API;
using Newtonsoft.Json;
using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkClient.Services;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    public class InterfaceController
    {
        public static Interfaces Interfaces = new Interfaces();
        public static dynamic InterfaceProperties = new { };
        public static ActiveInterfaces ActiveInterfaces = new ActiveInterfaces();
        public static IUserInterface FocusedInterface;
        /// <summary>
        /// Shows a user interface
        /// </summary>
        /// <remarks>Should only be used by the UserInterface abstract class.</remarks>
        /// <param name="interfaceItem">User Interface</param>
        public static void I_SHOW(IUserInterface interfaceItem)
        {

            var interfaceAttribute = interfaceItem.GetType().GetCustomAttribute<UserInterfaceAttribute>();
            if (interfaceAttribute != null)
            {
                if (interfaceAttribute.FOCUS_REQUIRED)
                {
                    SetNuiFocus(true, true);
                    Services.TaskService.InvokeUntilExpire(async () =>
                    {
                      /*  Debug.WriteLine("??");*/

                        if (IsControlJustReleased(0, 243))
                        {
                            /*Debug.WriteLine("Control Tilde");*/
                            interfaceItem.Hide();
                            FocusedInterface = null;
                            return true;
                        }
                            

                        return false;
                    });
                }
                SendNuiMessage(JsonConvert.SerializeObject(
                    new { 
                        method = "setVisible",
                        app=interfaceAttribute.HOOK,
                        data=true
                    }
                ));
            }
            else
            {
                throw new Exception("Interface does not have a valid connection attribute");
            }
        }
        /// <summary>
        /// Hides a user interface
        /// </summary>
        /// <remarks>Should only be used by the UserInterface abstract class.</remarks>
        /// <param name="interfaceItem">User Interface</param>
        public static void I_HIDE(IUserInterface userInterface)
        {
            var interfaceAttribute = userInterface.GetType().GetCustomAttribute<UserInterfaceAttribute>();
            if (interfaceAttribute != null)
            {
                if (interfaceAttribute.FOCUS_REQUIRED)
                {
                    SetNuiFocus(false, false);
                    FocusedInterface = null;
                }
                SendNuiMessage(JsonConvert.SerializeObject(
                    new
                    {
                        method = "setVisible",
                        app = interfaceAttribute.HOOK,
                        data = false
                    }
                ));
            }
            else
            {
                throw new Exception("Interface does not have a valid connection attribute");
            }
        }
        public static void SneakyHide(IUserInterface userInterface)
        {
            var interfaceAttribute = userInterface.GetType().GetCustomAttribute<UserInterfaceAttribute>();
            if (interfaceAttribute != null)
            {
                if (!ActiveInterfaces.ContainsKey(interfaceAttribute.HOOK)) return;
                ActiveInterfaces.Remove(interfaceAttribute.HOOK);
            }
            else
            {
                throw new Exception("Interface does not have a valid connection attribute");
            }
        }
        public static void SendNUI(string method, string app, object data)
        {
            SendNuiMessage(JsonConvert.SerializeObject(
                    new
                    {
                        method = method,
                        app = app,
                        data = data
                    }
                ));
        }

        static bool IsNuiHookRegistered = false;
        public static void HOOK<T>(string uiname,string cbType, Action<T> cb)
        {
            if(!IsNuiHookRegistered)
            {
                RegisterNuiCallbackType("pedata");
            }
            Framework.FrameworkController.HandleEvent("__cfx_nui:pedata", (IDictionary<string, object> data, CallbackDelegate cbx) =>
            {
                var ui = (string)data["ui"];
                var name = (string)data["name"];
                cbx(true);
                
                if (uiname == ui && cbType == name) {
                    try
                    {
                        
                        cb((T)data["data"]);
                    }catch (Exception ex)
                    {
                        DebugService.UnhandledException(ex);
                    }
                    
                }
            });
        }

        public static void RegisterNUIEvent<T>(string eventName, Action<IDictionary<string, object> , T> cb)
        {
            RegisterNuiCallbackType("pex_" + eventName);
            Framework.FrameworkController.HandleEvent("__cfx_nui:pex_" + eventName, (IDictionary<string, object> data, CallbackDelegate cbx) =>
            {
                DebugService.Watchpoint("PEX", data);
                cbx(true);
                try
                {
                    cb(data, (T)data["data"]);
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                    DebugService.DebugWarning("NUI_HOOK", $"Hook [{eventName}] failed: {ex.Message}");
                }
            });
        }

        public static void CONFIGURE(string uiname,string configName, object itm)
        {
            var data = JsonConvert.SerializeObject(
                    new
                    {
                        method = configName,
                        app = uiname,
                        data = itm
                    }
                );
            /*if (configName != "interacts")
                Debug.WriteLine(data);*/
            SendNuiMessage(data);
        }
        /// <summary>
        /// Shows a user interface
        /// </summary>
        /// <param name="name">The name of the interface</param>
        /// <returns>Whether the option was completed.</returns>
        public static bool ShowInterface(string name)
        {
            return ShowInterface(name, new { });
        }

        /// <summary>
        /// Shows an interface and sets some interface properties
        /// </summary>
        /// <param name="name">Name of the interface</param>
        /// <param name="setProps">Interface propertes</param>
        /// <returns>Whether the option was completed.</returns>
        public static bool ShowInterface(string name, dynamic setProps)
        {
            InterfaceProperties = setProps;
            if (ActiveInterfaces.ContainsKey(name)) return false;

            var instance = Activator.CreateInstance(Interfaces[name]);
            //DebugService.Watchpoint("SHOW_", setProps);
            ((UserInterface)instance).SetProperties(setProps);
            ActiveInterfaces.Add(name, (IUserInterface)instance);

            ActiveInterfaces[name].Show();



            return true;
        }
        /// <summary>
        /// Hides a user interface
        /// </summary>
        /// <param name="name">The name of the interface</param>
        /// <returns>Whether the option was completed.</returns>
        public static bool HideInterface(string name)
        {
            if (!ActiveInterfaces.ContainsKey(name)) return false;

            ActiveInterfaces[name].Destroy();
            ActiveInterfaces[name].Hide();
          

            if (!ActiveInterfaces.ContainsKey(name)) return false;

            
            try
            {
                ActiveInterfaces.Remove(name);

            }
            catch
            {

            }


            return true;
        }
        /// <summary>
        /// Hides a user interface
        /// </summary>
        /// <param name="name">The name of the interface</param>
        /// <returns>Whether the option was completed.</returns>
        public static void DestructInterface(string name)
        {
            if (!ActiveInterfaces.ContainsKey(name)) return;
            try
            {
                ActiveInterfaces.Remove(name);

            }
            catch
            {

            }
        }

        public static void REGISTER_INTERFACE(Type t)
        {
            var interfaceAttribute = t.GetCustomAttribute<UserInterfaceAttribute>();
            if (interfaceAttribute != null)
            {
                Interfaces.Add(interfaceAttribute.HOOK, t);
            }
        }
        /// <summary>
        /// Registers all interfaces in this client package.
        /// </summary>
        public static void REGISTER_INTERFACES()
        {
            var q = from t in Assembly.GetExecutingAssembly().GetTypes() where t.IsClass && t.GetCustomAttribute<UserInterfaceAttribute>() != null select t;
            q.ToList().ForEach(t => REGISTER_INTERFACE(t));
        }
    }

    public class Interfaces : Dictionary<string, Type> { }
    public class ActiveInterfaces : Dictionary<string, IUserInterface> { }
    public class ICALL
    {
        public string type;
        public object data;
    }

    public class ICALLS
    {
        struct INVOKE_INTERFACE_SHOW_HIDE
        {
            public string action;
            public string interfaceName;
        }

        public static ICALL SHOW_INTERFACE(string interfaceName)
        {
            INVOKE_INTERFACE_SHOW_HIDE S_HIDE;
            S_HIDE.action = "SHOW";
            S_HIDE.interfaceName = interfaceName;

            return new ICALL()
            {
                type = "SHOW_HIDE_INTERFACE",
                data = S_HIDE
            };
        }

        public static ICALL HIDE_INTERFACE(string interfaceName)
        {
            INVOKE_INTERFACE_SHOW_HIDE S_HIDE;
            S_HIDE.action = "HIDE";
            S_HIDE.interfaceName = interfaceName;

            return new ICALL()
            {
                type = "SHOW_HIDE_INTERFACE",
                data = S_HIDE
            };
        }
    }
}
