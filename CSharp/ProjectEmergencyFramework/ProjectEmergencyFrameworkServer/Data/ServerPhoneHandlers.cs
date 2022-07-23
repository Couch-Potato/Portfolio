using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class ServerPhoneHandlers
    {
        [Queryable("P_M_SEND")]
        public static void MessageSend(Query q, object i, Player px)
        {
            var dyn = (dynamic)i;
            string to = dyn.recipient;
            string message = dyn.data;


        }
    }
}
