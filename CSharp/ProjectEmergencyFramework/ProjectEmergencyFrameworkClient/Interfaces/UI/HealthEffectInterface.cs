using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("effects")]
    public class HealthEffectInterface : UserInterface
    {
        [Configuration("effectcfg")]
        public HealthEffectConfig config { get; set; } = new HealthEffectConfig();

        public void ShowEffectCaptions(bool shouldShow)
        {
            config.active = shouldShow;
            Update();
        }

        public async void AddEffect(string caption, string icon, string color)
        {
            config.items.Add(new HealthEffectItem() { caption = caption, color = color, icon = icon });
            ShowEffectCaptions(true);
            await BaseScript.Delay(5000);
            ShowEffectCaptions(false);
        }
        public void RemoveEffect(string caption, string icon, string color)
        {
            HealthEffectItem itemRef = null;
            foreach (var item in config.items)
            {
                if (item.color == color && item.caption == caption && item.icon  == icon)
                {
                    itemRef = item;
                }
            }
            config.items.Remove(itemRef);
            Update();
        }
    }
    public class HealthEffectConfig
    {
        public bool active { get; set; } = false;
        public List<HealthEffectItem> items { get; set; } = new List<HealthEffectItem>();
    }
    public class HealthEffectItem
    {
        public string caption { get; set; }
        public string icon { get; set; }
        public string color { get; set; }
    }
}
