using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Stretcher", "")]
    public class StretcherEquipable : Equipable
    {
        
        protected override async void OnEquip()
        {
            await Utility.AssetLoader.LoadModel("prop_ld_binbag_01");
            await Utility.AssetLoader.LoadAnimDict("anim@heists@box_carry@");
            var coords = Game.PlayerPed.Position;
            HealthService.Stretcher = CreateObject(GetHashKey("prop_ld_binbag_01"), coords.X, coords.Y, coords.Z, true, false, false);
            HealthService.isStretcherOut = true;
            NetworkRequestControlOfEntity(HealthService.Stretcher);
            AttachEntityToEntity(HealthService.Stretcher, Game.PlayerPed.Handle, GetPedBoneIndex(Game.PlayerPed.Handle, 28422), 0.0f, -0.6f, -1.43f, 180, 170, 90, false, false, false, true, 2, true);
            TaskService.InvokeUntilExpire(async () =>
            {
                if (!IsEntityPlayingAnim(Game.PlayerPed.Handle, "anim@heists@box_carry@", "idle", 3))
                {
                    TaskPlayAnim(Game.PlayerPed.Handle, "anim@heists@box_carry@", "idle", 8.0f, 8.0f, -1, 50, 0, false, false, false);
                }
                return !HealthService.isStretcherOut;
            });
        }
        protected override void OnUnEquip()
        {
            DetachEntity(HealthService.Stretcher, true, true);
            DeleteObject(ref HealthService.Stretcher);
            HealthService.isStretcherOut = false;
            if (HealthService.HasPersonInStretcher && !HealthService.IsStretcherOutOfCustody)
            {
                RPC.RPCService.RemoteCall(NetworkGetPlayerIndexFromPed(HealthService.PersonInStretcher.Handle), "EndStretcher", false);
            }
        }
    }
}
