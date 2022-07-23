using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Data.Model
{
    public class RPCall
    {
        public string CallName { get; set; }
        public int Target { get; set; }
        public object Data { get; set; }
    }
}
