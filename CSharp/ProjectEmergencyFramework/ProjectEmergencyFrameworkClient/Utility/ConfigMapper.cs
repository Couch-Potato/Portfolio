using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public class ConfigMapAttribute : Attribute
    {
        public string ConfigMap;
        public ConfigMapAttribute(string configMap)
        {
            ConfigMap = configMap;
        }
    }
    public static class ConfigCaster
    {
        public static object CastTo(this object input, Type to)
        {
            return Convert.ChangeType(input, to);
        }
    }

    public static class Configurator
    {
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void ConfigureAll()
        {
            foreach (var tps in Assembly.GetExecutingAssembly().GetTypes())
            {
                ConfigureType(tps);
            }
        }
        /// <summary>
        /// Loads a given configuration map
        /// </summary>
        /// <param name="mapName">The name of the configuration map</param>
        /// <returns>The configuration</returns>
        public static Task<object> DemandConfigurationMap(string mapName)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();

            FrameworkController.TriggerEvent("PE::CONFIG", mapName);

            Framework.FrameworkController.HandleEvent("PECR::CONFIG_" + mapName, new Action<object>((data) => {
                taskCompletionSource.SetResult(data);
            }));

            return taskCompletionSource.Task;
        }

        public static async Task<T> DemandConfigurationMap<T>(string mapName)
        {
            return (T)(await DemandConfigurationMap(mapName));
        }
        /// <summary>
        /// Binds a property to a configuration
        /// </summary>
        /// <param name="property"></param>
        public static async Task ConfigProperty(PropertyInfo property)
        {
            // Property has an active configuration
            if (property.GetCustomAttribute<ConfigMapAttribute>() != null)
            {
                var mapName = property.GetCustomAttribute<ConfigMapAttribute>().ConfigMap;
                try
                {
                    var mappedData = await DemandConfigurationMap(mapName);
                    var casted = mappedData.CastTo(property.PropertyType);
                    property.SetValue(null, casted, null);
                }catch (Exception ex)
                {
                   
                }
            }
        }

        public static void ConfigureType(Type t)
        {
            foreach (var prop in t.GetProperties())
            {
                ConfigProperty(prop);
            }
        }
        /// <summary>
        /// Configures a type object, but can be awaited so then it returns back when it is all completed.
        /// </summary>
        /// <param name="item">The object</param>
        public static async void ConfigureTypeAsync(Type t)
        {
            foreach (var prop in t.GetProperties())
            {
                await ConfigProperty(prop);
            }
        }
    }
}
