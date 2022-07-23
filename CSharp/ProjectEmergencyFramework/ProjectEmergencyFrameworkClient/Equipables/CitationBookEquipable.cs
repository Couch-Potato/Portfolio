using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Citation Book", "/assets/inventory/notepad_pencil.svg")]
    public class CitationBookEquipable:Equipable
    {
        int prop;
        int prop2;

        public override bool DisabledPrimary => true;
        protected override async void OnEquip()
        {
            await Utility.AssetLoader.LoadAnimDict("missheistdockssetup1clipboard@base");
            var coords = Game.PlayerPed.Position;
            prop = CreateObject(GetHashKey("prop_notepad_01"), coords.X, coords.Y, coords.Z, true, true, true);
            prop2 = CreateObject(GetHashKey("prop_pencil_01"), coords.X, coords.Y, coords.Z, true, true, true);
            AttachEntityToEntity(prop, Game.PlayerPed.Handle, GetPedBoneIndex(Game.PlayerPed.Handle, 18905), 0.1f, 0.02f, 0.05f, 10.0f, 0.0f, 0.0f, true, true, false, true, 1, true);
            AttachEntityToEntity(prop2, Game.PlayerPed.Handle, GetPedBoneIndex(Game.PlayerPed.Handle, 58866), 0.12f, 0.0f, 0.001f, -150.0f, 0.0f, 0.0f, true, true, false, true, 1, true);
            TaskPlayAnim(Game.PlayerPed.Handle, "missheistdockssetup1clipboard@base", "base", 8.0f, 1.0f, -1, 49, 0, false, false, false);
        }

        protected override async void OnUnEquip()
        {
            TaskPlayAnim(Game.PlayerPed.Handle, "missheistdockssetup1clipboard@base", "exit", 8.0f, 1.0f, -1, 49, 0, false, false, false);
            await BaseScript.Delay(100);
            ClearPedSecondaryTask(Game.PlayerPed.Handle);
            DetachEntity(prop, true, true);
            DeleteObject(ref prop);
            DetachEntity(prop2, true, true);
            DeleteObject(ref prop2);
        }

        protected override void OnPrimaryUp()
        {
            Interfaces.InterfaceController.ShowInterface("citationPad");
        }
    }
}
