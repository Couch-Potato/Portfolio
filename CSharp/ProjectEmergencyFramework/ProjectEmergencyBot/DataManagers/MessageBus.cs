using MongoDB.Driver;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyBot.DataManagers
{
    public delegate void MessageBusCall(dynamic data);
    public delegate void MessageBusQuery(dynamic data);


    public class MessageBusService
    {
        const MessageHost CURRENT_DOMAIN = MessageHost.Bot;

        public static Dictionary<string, MessageBusCall> CallHandlers = new Dictionary<string, MessageBusCall>();
        private static IMongoCollection<MessageBus> msgBus;
        public static async void BeginListen(IMongoCollection<MessageBus> dbWatch)
        {
            msgBus = dbWatch;
            foreach (var type in typeof(MessageBusService).Assembly.GetTypes())
            {
                if (type.IsSealed == false) continue;
                if (type.IsClass == false) continue;

                foreach (var method in type.GetRuntimeMethods())
                {
                    // Make sure the method is static
                    if (method.IsStatic == false) continue;

                    // Test for presence of the attribute
                    var attribute = method.GetCustomAttribute<RPCHandlerAttribute>();

                    if (attribute == null)
                        continue;

                    try
                    {
                        var del = (MessageBusCall)method.CreateDelegate(typeof(MessageBusCall));
                        CallHandlers.Add(attribute.Name, del);
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine($"{method.Name}: {ex.Message}");
                    }
                }
            }
            var pendingMessages = dbWatch.Find(msg => msg.Destination == CURRENT_DOMAIN).ToList();
            foreach (var message in pendingMessages)
            {
                if (!message.IsQuery)
                {
                    CallHandlers[message.Route](
                            JsonConvert.DeserializeObject<ExpandoObject>(message.Data)
                            );
                    dbWatch.DeleteOne(d => d.Id == message.Id);
                }
            }
            var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<MessageBus>>().Match(x => x.OperationType == ChangeStreamOperationType.Insert && x.FullDocument.Destination == CURRENT_DOMAIN);
            using (var cursor = await dbWatch.WatchAsync(pipeline))
            {
                await cursor.ForEachAsync(change =>
                {
                    if (!change.FullDocument.IsQuery)
                    {
                        CallHandlers[change.FullDocument.Route](
                            JsonConvert.DeserializeObject<ExpandoObject>(change.FullDocument.Data)
                            );
                    }
                    dbWatch.DeleteOne(d => d.Id == change.FullDocument.Id);
                    // process change event
                });
            }
        }
        public static void RPCCall(MessageHost dest, string name, dynamic data)
        {
            msgBus.InsertOne(new MessageBus()
            {
                Destination = dest,
                Author = CURRENT_DOMAIN,
                IsQuery = false,
                Data = JsonConvert.SerializeObject(data),
                Route = name
            });
        }
    }
    public class RPCHandlerAttribute : Attribute
    {
        public string Name { get; set; }
        public RPCHandlerType RPCHandlerType { get; set; }
        public RPCHandlerAttribute(string name, RPCHandlerType type = RPCHandlerType.Call)
        {
            RPCHandlerType = type;
            Name = name;
        }
    }
    public enum RPCHandlerType
    {
        Call,
        Query
    }
}
