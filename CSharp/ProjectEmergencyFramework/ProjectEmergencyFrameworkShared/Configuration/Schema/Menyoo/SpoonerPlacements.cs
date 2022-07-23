using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema.Menyoo
{
    [Serializable()]
    [XmlRoot("SpoonerPlacements")]
    public class SpoonerPlacements : List<Placement>
    {
    }
}
