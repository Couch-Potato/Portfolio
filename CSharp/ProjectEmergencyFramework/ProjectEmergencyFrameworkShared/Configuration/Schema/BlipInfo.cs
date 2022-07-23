using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class BlipInfo
    {
        public int Id { get; set; }
        public int Color { get; set; }
        public string Text { get; set; }
    }
    public class LocationBlipInfo : BlipInfo
    {
        public Location Location { get; set; }
    }
}
