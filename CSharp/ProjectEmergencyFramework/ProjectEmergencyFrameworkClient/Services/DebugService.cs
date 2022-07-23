using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public static class DebugService
    {
        public static List<string> OWNER_STACK = new List<string>();
        public static List<string> HANDLER_STACK = new List<string>();

        public static string CURRENT_THREAD_OWNER = "UNKNOWN";
        public static string CURRENT_THREAD_HANDLER = "UNKNOWN";
        public static void DebugCall(string itemType, string itemName)
        {
            Debug.WriteLine($"{itemType} | {itemName}");
        }
        public static void DebugWarning(string itemType, string itemName)
        {
            Debug.WriteLine($"WARNING! [{itemType}] | {itemName}");
        }
        public static void SetDebugOwner(string s)
        {
            if (CURRENT_THREAD_OWNER != "UNKNOWN")
            {
                OWNER_STACK.Add(CURRENT_THREAD_OWNER);
            }
            CURRENT_THREAD_OWNER = s;
            SetDebugHandler("UNKNOWN");

        }
        public static void Watchpoint(string name, object data)
        {
            Debug.WriteLine($"=========== WATCHPOINT HIT ===========");
            Debug.WriteLine($"FUNCTION:{new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType.Name}.{new System.Diagnostics.StackFrame(1).GetMethod().Name}");
            Debug.WriteLine($"NAME:    {name}");
            Debug.WriteLine($"OWNER:   {CURRENT_THREAD_OWNER}");
            Debug.WriteLine($"HANDLER: {CURRENT_THREAD_HANDLER}");
            Debug.WriteLine($"------------ STACK TRACE -------------");
            Debug.WriteLine($"{new System.Diagnostics.StackTrace().ToString()}");
            Debug.WriteLine($"--------------------------------------");
            Debug.WriteLine($"------------ OBJECT REF'D ------------");
            Debug.WriteLine(JsonConvert.SerializeObject(data));
            Debug.WriteLine($"======================================");
        }
        public static void SetDebugHandler(string s)
        {
            if (CURRENT_THREAD_HANDLER != "UNKNOWN")
            {
                HANDLER_STACK.Add(CURRENT_THREAD_HANDLER);
            }
            CURRENT_THREAD_HANDLER = s;
        }
        public static void ClearDebugOwner()
        {
            if (OWNER_STACK.Count > 0)
            {
                CURRENT_THREAD_OWNER = OWNER_STACK[OWNER_STACK.Count-1];
                OWNER_STACK.RemoveAt(OWNER_STACK.Count - 1);
            }else
            {
                CURRENT_THREAD_OWNER = "UNKNOWN";
            }
            
            ClearDebugHandler();
        }
        public static void ClearDebugHandler()
        {
            if (HANDLER_STACK.Count > 0)
            {
                CURRENT_THREAD_HANDLER = HANDLER_STACK[HANDLER_STACK.Count - 1];
                HANDLER_STACK.RemoveAt(HANDLER_STACK.Count - 1);
            }
            else
            {
                CURRENT_THREAD_HANDLER = "UNKNOWN";
            }
        }

        public static void HandleError()
        {
            
        }

        public static void UnhandledException(Exception e)
        {
            if (e is WorkaroundException) return;
            Debug.WriteLine($"=========== CRITICAL ERROR ===========");
            Debug.WriteLine($": {(e).Message}");
            Debug.WriteLine($"{(e).StackTrace}");
            Debug.WriteLine($"OWNER: {CURRENT_THREAD_OWNER}");
            Debug.WriteLine($"HANDLER: {CURRENT_THREAD_HANDLER}");
            Debug.WriteLine($"======================================");
            
        }
        private static void DebugLog(dynamic d)
        {
            var wb = new WebClient();
            wb.Headers.Add("Content-Type", "application/json");
            wb.Headers.Add("X-Sentry-Auth", $"Sentry sentry_version=7,sentry_timestamp={CharacterService.Timestamp().ToString()},sentry_client=pe-error/1.0,sentry_key=e70290bc7520481c9f399ed8748e2f02");

            wb.UploadString("https://e70290bc7520481c9f399ed8748e2f02@o1267209.ingest.sentry.io/6453303", "POST", JsonConvert.SerializeObject(d));
        }
    }
    public class WorkaroundException : Exception
    {

    }
}
