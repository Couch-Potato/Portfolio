using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public class HUDService
    {
        private static HelpTextInterface helpTextInterface;
        private static object _HUD_LOCK;
        private static bool isHudLocked = false;

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void InitHUD()
        {
            helpTextInterface = new HelpTextInterface();
            helpTextInterface.Show();
        }
        public static void ForceHUDUnlock()
        {
            isHudLocked = false;
        }

        public static void Lock(object o)
        {
            _HUD_LOCK = o;
            isHudLocked = true;
        }
        public static bool Unlock(object o)
        {
            isHudLocked = _HUD_LOCK == o;
            return !isHudLocked;
        }

        public static void ShowHelpText(string caption, string color, float duration = -1)
        {
            if (isHudLocked) return;
            helpTextInterface.ShowHelpText(caption, color, duration);
        }
        public static void SetHelpText(string caption)
        {
            if (isHudLocked) return;
            helpTextInterface.SetText(caption);
        }
        public static void HideHelpText()
        {
            if (isHudLocked) return;
            helpTextInterface.HideHelpText();
        }
    }
}
