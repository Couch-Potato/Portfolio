#define DEBUG
using CitizenFX.Core;
using System;
using static CitizenFX.Core.Native.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Configuration;
using Newtonsoft.Json;

namespace ProjectEmergencyFrameworkClient
{
    public class FrameworkController : BaseScript
    {
        public FrameworkController()
        {
            DebugService.HandleError();
            try
            {
                Utility.ExecutionRoutines.OnInitializedRoutine();
            }
            catch(Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
           
            Interfaces.InterfaceController.REGISTER_INTERFACES();
            EventHandlers["onClientResourceStart"] += new Action<string>(OnResourceStart);
        }
        public void HandleEvent(string ev, Action<IDictionary<string, object>, CallbackDelegate> a)
        {
            this.EventHandlers[ev] += a;
        }
        public void HandleEvent(string ev, Delegate d)
        {
            if (EventHandlers.ContainsKey(ev))
            {
                EventHandlers[ev] += d;
            }else
            {
                EventHandlers.Add(ev, d);
            }
        }
        public ExportDictionary Export()
        {
            return Exports;
        }
        public static bool DEBUG = false;
        public static bool DO_DEBUG_INTERACTS = false;
        
        private void OnResourceStart(string resourceName)
        {
#if DEBUG
            DEBUG = true;
#endif

            string characterOverride = GetConvar("PE_characterOverride", "false");
            string orgOverride = GetConvar("PE_orgOverride", "false");
            string runDevTools = GetConvar("PE_isDevServer", "false");
            string runNormalThread = GetConvar("PE_production", "true");
            DO_DEBUG_INTERACTS = GetConvar("PE_doDebugIntcs", "false") == "true";

            if (Framework.FrameworkController != null) return;
            //Debug.Write(Exports["pe_config"]);
            Framework.FrameworkController = this;
            //Utility.ConfigMapper.ConfigureAll();
            /*Interfaces.InterfaceController.ShowInterface("characterselector");*/
            if (characterOverride != "false" && runNormalThread == "true")
            {
                if (orgOverride != "false")
                    Services.CharacterService.SetCharacterQuietly(characterOverride, orgOverride);
                else
                    CharacterService.SetCharacterQuietly(characterOverride);
                //Interfaces.InterfaceController.ShowInterface("characterselector");
            }
            else if (runNormalThread == "true")
            {
                Interfaces.InterfaceController.ShowInterface("characterselector");
            }

            //Interfaces.InterfaceController.ShowInterface("vehicleselector");
            SetManualShutdownLoadingScreenNui(true);

            try
            {
                if (runNormalThread == "true")
                    Utility.ExecutionRoutines.OnResourceStartRoutine();
                if (runDevTools == "true")
                    Utility.ExecutionRoutines.DebugStartRoutine();
            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
            }

            try
            {
                if (runNormalThread == "true")
                    ConfigurationService.ConfigureCurrentSession("build.json");
            }catch(Exception ex)
            {
                DebugService.UnhandledException(ex);
            }
          
            Tick += Interfaces.UI.KeybindService.Tick;
            Tick += async () =>
            {
                try
                {
                    if (runNormalThread == "true")
                        Utility.ExecutionRoutines.ExecuteTickRoutine();
                    if (runDevTools == "true")
                        Utility.ExecutionRoutines.ExecuteDevTickRoutine();
                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
               
            };

            SetMaxWantedLevel(0);

            Game.Player.DispatchsCops = false;
            Game.Player.IgnoredByPolice = true;
            Game.Player.IgnoredByEveryone = true;

            //DEBUG INTERACTS HAVE BEEN MOVED TO Debugging.DebugInteracts.MakeDevInteracts()
            //THIS IS SO THAT THEY DO NOT GET EXPOSED ON PRODUCTION LEVEL BUILDS


        }
        public ExportDictionary EXP => Exports;

    }
    public class Framework
    {
        public static FrameworkController FrameworkController;
    }
}
