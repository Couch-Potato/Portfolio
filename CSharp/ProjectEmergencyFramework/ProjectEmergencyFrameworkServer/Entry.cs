using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer
{
    public class Entry : BaseScript
    {
        public Entry()
        {
            Data.DatabaseClient.Connect();
            
        }
    }
}
