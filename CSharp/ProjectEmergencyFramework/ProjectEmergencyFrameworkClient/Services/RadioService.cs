using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Services
{
    public enum PresetRadioFrequency
    {
        County,
        Metro,
        TAC1,
        TAC2,
        Tow,
        Medic
    }
    public static class VoiceService
    {
        public static Interfaces.UI.Radio RadioInterface;
        public static void SetRadioFrequency(string freq)
        {
            QueryService.QueryConcrete<bool>("V_JOIN_FREQ", freq);
        }
        public static void EndRadio()
        {
            QueryService.QueryConcrete<bool>("V_END_FREQ", false);
        }

       
        public static void RadioServiceInit()
        {
            RadioInterface = new Interfaces.UI.Radio();
            RadioInterface.Show();
        }
    }
}
