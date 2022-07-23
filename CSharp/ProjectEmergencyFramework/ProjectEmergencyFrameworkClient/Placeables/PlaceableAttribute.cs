using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Placeables
{
    public class PlaceableAttribute :Attribute
    {
        public string Name { get; set; }
        public PlaceableAttribute(string name)
        {
            Name = name;
        }
    }
}
