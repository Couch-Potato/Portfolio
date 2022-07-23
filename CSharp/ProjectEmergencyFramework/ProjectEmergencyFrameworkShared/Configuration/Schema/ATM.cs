using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class ATM
    {
        public Location Location { get; set; } = new Location();
        public string Name { get; set; }
        public string Memo { get; set; }
    }
}
