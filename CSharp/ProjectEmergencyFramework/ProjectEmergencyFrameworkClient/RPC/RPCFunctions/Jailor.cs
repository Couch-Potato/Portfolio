using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.RPC.RPCFunctions
{
    public static class Jailor
    {
        [RPCQuery("GetMugshot")]
        public static void GetMugshot(Query q, object data)
        {
            Task.Run(async () =>
            {
                var mugshot = await HeadshotService.GetHeadshotOfPed(Game.PlayerPed.Handle);
                q.Reply(mugshot);
                
            });
        }
        [RPCQuery("GetGSR")]
        public static void GetGSR(Query q, object data)
        {
            if (CharacterService.Timestamp() > CharacterService.LastShotTime + (60 * 15))
            {
                q.Reply(false);
            }
            q.Reply(true);
        }

        [RPCQuery("GET_ID")]
        public static void GetId(Query q, object data)
        {
            q.Reply(CharacterService.CurrentCharacter.Id);
        }

        [RPCFunction("SHOW_PLEA")]
        public static void ShowPlea(dynamic a)
        {
            dynamic d = Utility.CrappyWorkarounds.JSONDynamicToExpando(a);
            Interfaces.InterfaceController.ShowInterface("pleaBargain", new
            {
                arrestId = d.arrestId
            });
        }

        [Queryable("INCARCERATE")]
        public static void DoIncarcerate(Query q, object data)
        {
            Interfaces.InterfaceController.HideInterface("pleaBargain");
            CharacterService.DoBeginIncarceration();


            q.Reply(true);
        }
    }
}
