using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public class ClassDiscovery
    { 
        public static List<DiscoveredItem> DiscoverWithAttribute<T>() where T : Attribute
        {
            var lst = new List<DiscoveredItem>();
            foreach (var typ in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typ.GetCustomAttribute<T>() != null)
                {
                    lst.Add(new DiscoveredItem()
                    {
                        InnerType = typ
                    });
                }
            }
            return lst;
        }
    }
    public class DiscoveredItem
    {
        public Type InnerType;
        public T ConstructAs<T>()
        {
            return (T)Activator.CreateInstance(InnerType);
        }
        public T GetAttribute<T>() where T : Attribute
        {
            return InnerType.GetCustomAttribute<T>();
        }
    }
}
