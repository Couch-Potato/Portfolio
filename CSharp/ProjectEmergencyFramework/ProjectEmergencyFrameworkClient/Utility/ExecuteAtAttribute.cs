using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public enum ExecutionStage
    {
        OnResourceStart,
        Initialized,
        Custom, 
        Tick,
        DevToolsStart,
        DevToolsTick
    }
    public class ExecuteAtAttribute : Attribute
    {
        public ExecutionStage ExecutionStage;
        public string EventHookName;
        public ExecuteAtAttribute(ExecutionStage stage)
        {
            ExecutionStage = stage;
        }

        public ExecuteAtAttribute(string hook)
        {
            ExecutionStage = ExecutionStage.Custom;
            EventHookName = hook;
        }
    }
    public static class ExecutionRoutines
    {
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void HandleCustomRoutines()
        {
            //lel no.
        }

        public static void OnResourceStartRoutine()
        {
            DebugService.DebugCall("ROUTINE", "Executing routine: ONRESOURCESTART");
            DebugService.SetDebugOwner("ROUT::RESOURCE_START");

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ExecuteAtAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ExecuteAtAttribute>();
                            if (attb.ExecutionStage == ExecutionStage.OnResourceStart)
                            {
                                var dg = (Action)method.CreateDelegate(typeof(Action));
                                //Debug.WriteLine("Executing routine method: " + method.Name);
                                DebugService.DebugCall("RESOURCE_START", method.Name);
                                DebugService.SetDebugHandler(method.Name);
                                dg();
                                DebugService.ClearDebugHandler();

                            }
                        }
                    }
                }
            }
            DebugService.ClearDebugOwner();
        }

        public static void DebugStartRoutine()
        {
            DebugService.DebugCall("ROUTINE", "Executing routine: ONDEBUGSTART");

            DebugService.SetDebugOwner("ROUT::DEVTOOLS_START");

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ExecuteAtAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ExecuteAtAttribute>();
                            if (attb.ExecutionStage == ExecutionStage.DevToolsStart)
                            {
                                var dg = (Action)method.CreateDelegate(typeof(Action));
                                //Debug.WriteLine("Executing routine method: " + method.Name);
                                DebugService.DebugCall("DEVTOOLS_START", method.Name);
                                DebugService.SetDebugHandler(method.Name);
                                dg();
                                DebugService.ClearDebugHandler();

                            }
                        }
                    }
                }
            }
            DebugService.ClearDebugOwner();
        }

        public static event Action TickHappen;
        public static event Action DevTickHappen;

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void InitializeTickRoutine()
        {
            DebugService.DebugCall("ROUTINE_TICK", "Discovering tick routines...");

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ExecuteAtAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ExecuteAtAttribute>();
                            if (attb.ExecutionStage == ExecutionStage.Tick)
                            {
                                var dg = (Action)method.CreateDelegate(typeof(Action));
                                DebugService.DebugCall("ROUTINE_TICK", "Discovered tick routine method: " + method.Name);
                                TickHappen += () =>
                                {
                                    try
                                    {
                                        DebugService.SetDebugHandler(method.Name);
                                        dg();
                                        DebugService.ClearDebugHandler();

                                    }
                                    catch (Exception ex)
                                    {
                                        DebugService.UnhandledException(ex);
                                        DebugService.ClearDebugHandler();

                                    }
                                };
                            }
                        }
                    }
                }
            }
        }
        [ExecuteAt(ExecutionStage.Initialized)]

        public static void InitializeDevTickRoutine()
        {
            DebugService.DebugCall("DEV_ROUTINE_TICK", "Discovering tick routines...");

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ExecuteAtAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ExecuteAtAttribute>();
                            if (attb.ExecutionStage == ExecutionStage.DevToolsTick)
                            {
                                var dg = (Action)method.CreateDelegate(typeof(Action));
                                DebugService.DebugCall("DEV_ROUTINE_TICK", "Discovered routine method: " + method.Name);
                                DevTickHappen += () =>
                                {
                                    try
                                    {
                                        DebugService.SetDebugHandler(method.Name);
                                        dg();
                                        DebugService.ClearDebugHandler();

                                    }
                                    catch (Exception ex)
                                    {
                                        DebugService.UnhandledException(ex);
                                        DebugService.ClearDebugHandler();

                                    }
                                };
                            }
                        }
                    }
                }
            }
        }

        static bool stopped = false;
        public static void ExecuteTickRoutine()
        {
            try
            {
                DebugService.SetDebugOwner("TICK");
                if(!stopped)
                    TickHappen?.Invoke();
                DebugService.ClearDebugOwner();
            }
            catch (Exception ex)
            {
                stopped = true;
                DebugService.UnhandledException(ex);
                //Debug.WriteLine("Tick error:" + ex.Message);
            }
            
        }
        public static void ExecuteDevTickRoutine()
        {
            try
            {
                DebugService.SetDebugOwner("DEVTICK");
                if (!stopped)
                    DevTickHappen?.Invoke();
                DebugService.ClearDebugOwner();
            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
                stopped = true;
                //Debug.WriteLine("DEV Tick error:" + ex.Message);
            }

        }
        public static void OnInitializedRoutine()
        {
            DebugService.DebugCall("ROUTINE", "Starting init routine...");
            DebugService.SetDebugOwner("ROUT::INITIALIZE");
            
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                foreach (var method in type.GetMethods())
                {
                    if (method.IsStatic)
                    {
                        if (method.GetCustomAttribute<ExecuteAtAttribute>() != null)
                        {
                            var attb = method.GetCustomAttribute<ExecuteAtAttribute>();
                            if (attb.ExecutionStage == ExecutionStage.Initialized)
                            {
                                var dg = (Action)method.CreateDelegate(typeof(Action));
                                DebugService.SetDebugHandler(method.Name);
                                DebugService.DebugCall("INITIALIZE", method.Name);
                                //Debug.WriteLine("Executing routine method: " + method.Name);
                                try
                                {
                                    dg();
                                }
                                catch (Exception ex)
                                {
                                    DebugService.UnhandledException(ex);
                                   
                                }
                                
                            }
                        }
                    }
                }
            }
            DebugService.ClearDebugOwner();
        }
    }
}
