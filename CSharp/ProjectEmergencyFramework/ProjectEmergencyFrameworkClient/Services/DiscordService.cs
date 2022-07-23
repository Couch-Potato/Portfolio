using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services
{
    public static class DiscordService
    {
        const string APPLICATION_ID = "986031519485268078";
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void InitializeDiscordRPC()
        {
            SetDiscordAppId(APPLICATION_ID);
            SetDiscordRichPresenceAsset("fivem");
            SetDiscordRichPresenceAssetText("Playing Project Emergency");
            SetRichPresence("Playing Project Emergency");
        }
    }
}
