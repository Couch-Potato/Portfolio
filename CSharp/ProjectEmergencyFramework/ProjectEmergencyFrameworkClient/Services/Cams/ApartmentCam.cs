using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services.Cams
{
    internal class ApartmentCam : CameraOperator
    {
        Camera c;
        private Vector3 initPos;
        protected override void OnStart()
        {
            initPos = Game.PlayerPed.Position;
            Game.PlayerPed.IsInvincible = true;
            Game.PlayerPed.IsVisible = false;
            Game.PlayerPed.IsPositionFrozen = true;
            c = new Camera(CreateCam("DEFAULT_SCRIPTED_CAMERA", true));
            c.Position = Modifiers.position;
            c.PointAt(Modifiers.point);
            RenderScriptCams(true, false, 0, false, false);

            base.OnStart();
        }
        protected override void OnStop()
        {
            Game.PlayerPed.IsInvincible = false;
            Game.PlayerPed.IsVisible = true;
            Game.PlayerPed.IsPositionFrozen = false;
            c.IsActive = false;
            Game.PlayerPed.Position = initPos;

            RenderScriptCams(false, false, 0, false, false);
            c.Delete();

            base.OnStop();
        }
        protected override void OnTick()
        {
            c.Position = Modifiers.position;
            Game.PlayerPed.Position = Modifiers.position;
            c.PointAt(Modifiers.point);
        }
    }
}
