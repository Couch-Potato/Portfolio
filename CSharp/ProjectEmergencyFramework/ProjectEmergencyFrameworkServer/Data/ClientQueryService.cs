using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using CitizenFX.Core;
using System.Reflection;
using ProjectEmergencyFrameworkServer.Utility;
using Newtonsoft.Json;

namespace ProjectEmergencyFrameworkServer.Data
{
    public delegate void QueryDelegate(Query q, object value, Player plyr);
    public class ClientQueryService:BaseScript
    {
        public static ClientQueryService Instance { get; private set; }
        bool started = false;
        public ClientQueryService()
        {

            EventHandlers["onResourceStart"] += new Action<string>(Setup);
            ClientQueryService.Instance = this;
            
        }
        private Dictionary<string, QueryDelegate> Queryables = new Dictionary<string, QueryDelegate>();
        private void Setup(string s)
        {
            if (started) return;
            started = false;
            EventHandlers["PE::QUERY"] += new Action<Player, string, object>(HandleQuery);
            Framework.FrameworkController = this;
            Queryables.Clear();
            foreach (var type in typeof(ClientQueryService).Assembly.GetTypes())
            {
                if (type.IsSealed == false) continue;
                if (type.IsClass == false) continue;

                foreach (var method in type.GetRuntimeMethods())
                {
                    // Make sure the method is static
                    if (method.IsStatic == false) continue;

                    // Test for presence of the attribute
                    var attribute = method.GetCustomAttribute<QueryableAttribute>();

                    if (attribute == null)
                        continue;

                    try
                    {
                        var del = (QueryDelegate)method.CreateDelegate(typeof(QueryDelegate));
                        Queryables.Add(attribute.QueryName, del);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"{method.Name}: {ex.Message}");
                    }



                }
            }
        }
        internal void RegisterEvent(string name, Delegate d)
        {
            EventHandlers[name] += d;
        }
        private void HandleQuery([FromSource] Player px, string method, object tp)
        {
            Debug.WriteLine($"CALL->IN | {method}");
            Queryables[method](new Query(method, new Query.HandleReply((object rply) =>
            {
                Debug.WriteLine($"CALL->REPLY | {method}");
                TriggerClientEvent(px, "PER::" + method, rply);
            })), tp, px);
        }
        public PlayerList AllPlayers => Players;
        public ExportDictionary Exported => base.Exports;
    }
    public static class Framework
    {
        public static ClientQueryService FrameworkController;
    }
    public class Query
    {
        public delegate void HandleReply(object reply);
        private HandleReply ReplyHandler;
        private string QItem;
        public Query(string item, HandleReply rx)
        {
            QItem = item;
            ReplyHandler = rx;
        }
        public void Reply(object reply)
        {
            ReplyHandler(reply);
        }
    }
    public static class QueryService
    {
        public static Task<T> Query<T>(int client, string qName, params object[] itm)
        {
            PlayerList pl = new PlayerList();
            var taskCompletionSource = new TaskCompletionSource<T>();
            Debug.WriteLine($"CALL->OUT | {qName}");
            if (itm != null)
            {
                if (itm.Length == 0)
                {
                    if (client == -1)
                    {
                        ClientQueryService.TriggerClientEvent(qName, JsonConvert.SerializeObject(itm));
                    }else
                    {
                        ClientQueryService.TriggerClientEvent(pl[client], "RDIR::PE:S::QUERY", qName, JsonConvert.SerializeObject(itm));

                    }

                }
                else if (itm.Length == 1)
                {
                    if (client == -1)
                    {
                        ClientQueryService.TriggerClientEvent("RDIR::PE:S::QUERY", qName, JsonConvert.SerializeObject(itm[0]));

                    }
                    else
                    {
                        ClientQueryService.TriggerClientEvent(pl[client], "RDIR::PE:S::QUERY", qName, JsonConvert.SerializeObject(itm[0]));


                    }

                }
                else
                {
                    if (client == -1)
                    {
                        ClientQueryService.TriggerClientEvent(qName, JsonConvert.SerializeObject(itm));
                    }
                    else
                    {
                        ClientQueryService.TriggerClientEvent(pl[client], "RDIR::PE:S::QUERY", qName, JsonConvert.SerializeObject(itm));

                    }
                    /*                    FrameworkController.TriggerServerEvent("PE::QUERY", qName, itm);*/
                }
            }

            bool isCompleted = false;

            ClientQueryService.Instance.RegisterEvent("PE::S::R::" + qName, new Action<T>((T item) =>
            {
                Debug.WriteLine($"CALL->RET | {qName}");
                if (!isCompleted)
                    taskCompletionSource.TrySetResult(item);
                isCompleted = true;
            }));
           
            return taskCompletionSource.Task;
        }
        public static async Task<T> QueryConcrete<T>(int client, string qName, params object[] itm)
        {
            var dyn = await Query<dynamic>(client, qName, itm);
            return CrappyWorkarounds.ShittyFiveMDynamicToConcrete<T>(dyn);
        }
    }
}