using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("helpText")]
    public class HelpTextInterface : UserInterface
    {
        [Configuration("htConfig")]
        public HelpTextConfig config { get; set; } = new HelpTextConfig();
        
        public void SetText(string text)
        {
            config.caption = text;
            Update();
        }
        public async void ShowHelpText(string caption, string color, float duration = -1)
        {
            config.caption = caption;
            config.color = color;
            config.showing = true;
            await UpdateAsync();
            if (duration > 0)
            {
                await BaseScript.Delay((int)(duration * 1000));
                config.showing = false;
                await UpdateAsync();
            }

        }
        public void HideHelpText()
        {
            config.showing = false;
            Update();
        }
        
    }
    public class HelpTextConfig
    {
        public string color { get; set; } = "";
        public string caption { get; set; } = "";
        public bool showing { get; set; } = false;
    }
}
