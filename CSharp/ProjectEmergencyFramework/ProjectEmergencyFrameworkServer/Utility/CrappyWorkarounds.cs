using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Utility
{
    public static class CrappyWorkarounds
    {
        /// <summary>
        /// A shitty workaround to convert a dynamic type to a concrete one
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="item">The dynamic</param>
        /// <returns>The concrete</returns>
        public static T ShittyFiveMDynamicToConcrete<T>(dynamic item)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item));
        }

        public static List<T> ConcreteifyList<T>(object HOLDER)
        {
            var list = new List<T>();
            var convd = (List<object>)HOLDER;
            foreach (var x in convd)
            {
                list.Add(ShittyFiveMDynamicToConcrete<T>(x));
            }
            return list;
        }
        public static ExpandoObject JSONDynamicToExpando(object item)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(item));
        }
        public static void PrintExpando(List<ExpandoObject> ItemList)
        {
            foreach (var item in ItemList)
            {
                IDictionary<string, object> propertyValues = item;

                foreach (var property in propertyValues.Keys)
                {
                    Debug.WriteLine(String.Format("{0} : {1}", property, propertyValues[property]));
                }
            }
        }
        public static bool HasProperty(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }

    }
}
