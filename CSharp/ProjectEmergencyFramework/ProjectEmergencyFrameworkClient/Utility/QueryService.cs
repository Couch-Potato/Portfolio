using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CitizenFX.Core.Native.API;
using System.Threading.Tasks;
using System.Reflection;
using ProjectEmergencyFrameworkClient.Services;
using Newtonsoft.Json;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public delegate void QueryDelegate(Query q, object value);
    public delegate void Anonymous(object ob);
    public static class QueryService
    {
        private static Dictionary<string, QueryDelegate> Queryables = new Dictionary<string, QueryDelegate>();
        [Obsolete("Use QueryConcrete<t>() and not Query(). This one is not coded too properly.", true)]
        public static Task<object> Query(string qName, params object[] itm)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            bool isCompleted = false;
            Framework.FrameworkController.HandleEvent("PER::" + qName, new Anonymous((object item) =>
            {
                if (!isCompleted)
                    taskCompletionSource.TrySetResult(item.ToString());
                isCompleted = true;
            }));
            return taskCompletionSource.Task;
        }
        /// <summary>
        /// Queries the server for something
        /// </summary>
        /// <typeparam name="T">The type of object to be returned</typeparam>
        /// <param name="qName">The type of query to call</param>
        /// <param name="itm">Any query parameters you have</param>
        /// <returns>A dynamic of the query</returns>
        /// <remarks>DO NOT USE UNLESS YOU CONVERT THE OBJECT WITH CRAPPYWORKAROUNDS</remarks>
        [Obsolete("Do not use this query. Use QuerryConcrete instead. I can assure you this probably does not do what you need it to do.")]
        public static Task<T> Query<T>(string qName, params object[] itm)
        {
            DebugService.DebugCall("QUERY->OUT", qName);
            var taskCompletionSource = new TaskCompletionSource<T>();

            if (itm != null)
            {
                if (itm.Length == 0)
                {
                    FrameworkController.TriggerServerEvent("PE::QUERY", qName, itm);
                } else if (itm.Length == 1)
                {
                    FrameworkController.TriggerServerEvent("PE::QUERY", qName, itm[0]);
                }
                else
                {
                    FrameworkController.TriggerServerEvent("PE::QUERY", qName, itm);
                }
            }

            bool isCompleted = false;
            Framework.FrameworkController.HandleEvent("PER::" + qName, new Action<T>((T item) =>
            {
                DebugService.DebugCall("QUERY->RET", qName);
                if (!isCompleted)
                    taskCompletionSource.TrySetResult(item);
                isCompleted = true;
            }));
            return taskCompletionSource.Task;
        }
        /// <summary>
        /// Performs a query and returns a concrete response.
        /// </summary>
        /// <typeparam name="T">The concrete class</typeparam>
        /// <param name="qName">The query</param>
        /// <param name="itm">Query parameters</param>
        /// <returns>The concrete result of the query</returns>
        public static async Task<T> QueryConcrete<T>(string qName, params object[] itm)
        {
            var dyn = await Query<dynamic>(qName, itm);
            return CrappyWorkarounds.ShittyFiveMDynamicToConcrete<T>(dyn);
        }
        /// <summary>
        /// Queries for a list
        /// </summary>
        /// <typeparam name="T">Type of list to query</typeparam>
        /// <param name="qName">Name of the query call to run</param>
        /// <param name="itm">Parameters</param>
        /// <returns>The list</returns>
        public static async Task<List<T>> QueryList<T>(string qName, params object[] itm)
        {
            var dyn = await Query<List<object>>(qName, itm);

            var list = new List<T>();

            foreach (var vh in dyn)
            {
                list.Add(Utility.CrappyWorkarounds.ShittyFiveMDynamicToConcrete<T>(vh));
            }
            return list;
        }
        [ExecuteAt(ExecutionStage.Initialized)]
        public static void LoadQueryHandlers()
        {
            foreach (var type in typeof(QueryService).Assembly.GetTypes())
            {
                if (type.IsSealed == false) continue;
                if (type.IsClass == false) continue;

                foreach (var method in type.GetMethods())
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
                        DebugService.UnhandledException(ex);
                    }



                }
            }
        }
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void SetupQueryRX()
        {
            Framework.FrameworkController.HandleEvent("RDIR::PE:S::QUERY", new Action<string, object>((string method, object item) =>
            {
                DebugService.DebugCall("QUERY->IN", method);
                DebugService.SetDebugOwner("QUERY->IN");
                DebugService.SetDebugHandler(method);
                try
                {
                    
                    Queryables[method](new Query(method, new Query.HandleReply((object rply) =>
                    {
                        DebugService.DebugCall("QUERY->REP", method);
                        DebugService.ClearDebugOwner();
                        FrameworkController.TriggerServerEvent("PE::S::R::" + method, rply);
                    })), JsonConvert.DeserializeObject((string)item));

                }
                catch (Exception ex)
                {
                    DebugService.UnhandledException(ex);
                }
                DebugService.ClearDebugHandler();

            }));

/*
           EventHandlers["PE::QUERY"] += new Action<Player, string, object>(HandleQuery);
            Framework.FrameworkController = this;*/
           
        }
    }
    
    public class QueryScript : BaseScript
    {
        public QueryScript()
        {
            
        }
    }
    public class Query
    {
        public delegate void HandleReply(object reply);
        private HandleReply ReplyHandler;
        public string QItem;
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
    public class QueryableAttribute : Attribute
    {
        public string QueryName;
        public QueryableAttribute(string name)
        {
            QueryName = name;
        }
    }
}
