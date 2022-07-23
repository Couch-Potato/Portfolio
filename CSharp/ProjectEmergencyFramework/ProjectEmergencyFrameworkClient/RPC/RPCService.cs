using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.RPC
{
    public delegate void RPCQueryDelegate(Query q, object value);
    public delegate void RPCCallDelegate(object value);
    public static class RPCService
    {
        static Dictionary<string, RPCCallDelegate> calls = new Dictionary<string, RPCCallDelegate>();
        static Dictionary<string, RPCQueryDelegate> queries = new Dictionary<string, RPCQueryDelegate>();

        public static void RemoteCall(int playerId, string callName, object data)
        {
            DebugService.DebugCall("RPCC->OUT", callName);
            QueryService.QueryConcrete<bool>("DO_REMOTE_CALL", new ProjectEmergencyFrameworkShared.Data.Model.RPCall() { CallName = callName, Data = data, Target = playerId});
        }
        public static async Task<T> RemoteQuery<T>(int playerId, string callName, object data)
        {
            DebugService.DebugCall("RPCQ->OUT", callName);
            return await QueryService.QueryConcrete<T>("DO_REMOTE_QUERY", new ProjectEmergencyFrameworkShared.Data.Model.RPCall() { CallName = callName, Data = data, Target = playerId });
        }

        [Queryable("RPCALL")]
        public static void callRouter(Query q, object value)
        {
            DebugService.SetDebugOwner("RPCC->IN");
            dynamic callData = Utility.CrappyWorkarounds.JSONDynamicToExpando(value);
            q.Reply(true);
            try
            {
                DebugService.SetDebugHandler(q.QItem);
                DebugService.DebugCall("RPCC->IN", q.QItem);
                DebugService.Watchpoint("CALL IN", callData);
                calls[callData.CallName](callData.Data);
                DebugService.ClearDebugHandler();

            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
                DebugService.DebugWarning("RPC", "Remote Procedure Query: " + callData.CallName + " failed. " + ex.Message);
                DebugService.ClearDebugHandler();

            }
            DebugService.ClearDebugOwner();
        }


        [Queryable("RPQUERY")]
        public static void queryRouter(Query q, object value)
        {
            DebugService.SetDebugOwner("RPCQ->IN");
            dynamic callData = Utility.CrappyWorkarounds.JSONDynamicToExpando(value);
            try
            {
                DebugService.SetDebugHandler(q.QItem);
                DebugService.DebugCall("RPCQ->IN", q.QItem);
                queries[callData.CallName](q, callData.Data);
                DebugService.ClearDebugHandler();

            }
            catch (Exception ex)
            {
                DebugService.UnhandledException(ex);
                DebugService.DebugWarning("RPC", "Remote Procedure Query: " + callData.CallName + " failed. " + ex.Message);
                
                DebugService.ClearDebugHandler();

            }
            DebugService.ClearDebugOwner();
        }

        [ExecuteAt(ExecutionStage.Initialized)]
        public static void doQueryInit()
        {
            foreach (var type in typeof(RPCService).Assembly.GetTypes())
            {
                if (type.IsSealed == false) continue;
                if (type.IsClass == false) continue;

                foreach (var method in type.GetMethods())
                {
                    // Make sure the method is static
                    if (method.IsStatic == false) continue;

                    // Test for presence of the attribute
                    var attribute = method.GetCustomAttribute<RPCFunctionAttribute>();

                    if (attribute != null)
                    {
                        try
                        {
                            var del = (RPCCallDelegate)method.CreateDelegate(typeof(RPCCallDelegate));
                            calls.Add(attribute.QueryName, del);
                        }
                        catch (Exception ex)
                        {
                            DebugService.UnhandledException(ex);
                        }

                    }

                    var attribute2 = method.GetCustomAttribute<RPCQueryAttribute>();

                    if (attribute2 == null)
                        continue;

                    try
                    {
                        var del = (RPCQueryDelegate)method.CreateDelegate(typeof(RPCQueryDelegate));
                        
                        queries.Add(attribute2.QueryName, del);
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
