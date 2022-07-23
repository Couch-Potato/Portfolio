using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class CrappyWorkarounds
    {
        /// <summary>
        /// A shitty workaround to convert a dynamic type to a concrete one
        /// </summary>
        /// <typeparam name="T">The object type</typeparam>
        /// <param name="item">The dynamic</param>
        /// <returns>The concrete</returns>
        /// <remarks>FUCK FIVEM, SHIT IS FILLED WITH RETARDS</remarks>
        public static T ShittyFiveMDynamicToConcrete<T>(dynamic item)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(item));
        }

        public static bool HasProperty(dynamic settings, string name)
        {
            if (settings is ExpandoObject)
                return ((IDictionary<string, object>)settings).ContainsKey(name);

            return settings.GetType().GetProperty(name) != null;
        }
        public static ExpandoObject JSONDynamicToExpando(object item)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(item));
        }
        public static ExpandoObject ConvertToExpando(object obj)
        {
            //Get Properties Using Reflections
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = obj.GetType().GetProperties(flags);

            //Add Them to a new Expando
            ExpandoObject expando = new ExpandoObject();
            var byName = (IDictionary<string, object>)obj;
            foreach (var kvp in byName)
            {
                AddProperty(expando, kvp.Key, kvp.Value);
            }

            return expando;
        }
        public static void AddProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            //Take use of the IDictionary implementation
            var expandoDict = expando as IDictionary<String, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}
