using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class Component
    {
        public List<Script> Scripts = new List<Script>();

        public List<ComponentProperty> Properties = new List<ComponentProperty>();
    }
    public class ComponentProperty
    {
        string Name;
        object Value;
    }
}
