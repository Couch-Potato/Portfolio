using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class RPCallHandler
    {
        [Queryable("DO_REMOTE_CALL")]
        public static void RemoteCall(Query q, object i, Player px)
        {  
           var call = (dynamic)i;
           QueryService.Query<bool>(call.Target, "RPCALL", call);
            q.Reply(true);
        }

        [Queryable("DO_REMOTE_QUERY")]
        public static async void RemoteQuery(Query q, object i, Player px)
        {

            var call = (dynamic)i;
            
                var result = await QueryService.Query<object>(call.Target, "RPQUERY", call);
                q.Reply(result);
            

        }
    }
}
