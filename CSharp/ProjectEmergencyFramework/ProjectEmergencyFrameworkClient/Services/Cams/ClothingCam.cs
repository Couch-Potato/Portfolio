using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Services.Cams
{
    public class ClothingCam : CameraOperator
    {
        public const int FULL_BODY = 0;
        public const int HEAD = 1;
        public const int UPPER = 2;
        public const int LOWER = 3;
        public const int SHOES = 4;
        public const int LOWER_ARMS = 5;
        public const int FULL_ARMS = 6;
        internal static List<KeyValuePair<Vector3, Vector3>> CameraOffsets { get; } = new List<KeyValuePair<Vector3, Vector3>>()
        {
            // Full body
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 2.8f, 0.3f), new Vector3(0f, 0f, 0f)),

            // Head level
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 0.9f, 0.65f), new Vector3(0f, 0f, 0.6f)),

            // Upper Body
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 1.4f, 0.5f), new Vector3(0f, 0f, 0.3f)),

            // Lower Body
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 1.6f, -0.3f), new Vector3(0f, 0f, -0.45f)),

            // Shoes
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 0.98f, -0.7f), new Vector3(0f, 0f, -0.90f)),

            // Lower Arms
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 0.98f, 0.1f), new Vector3(0f, 0f, 0f)),

            // Full arms
            new KeyValuePair<Vector3, Vector3>(new Vector3(0f, 1.3f, 0.35f), new Vector3(0f, 0f, 0.15f)),
        };
        internal static Vector2[] offsets = new Vector2[] {
            new Vector2(2.2f, -1f),
            new Vector2(0.7f, -0.45f),
            new Vector2(1.35f, -0.4f),
            new Vector2(1.0f, -0.4f),
            new Vector2(0.9f, -04f),
            new Vector2(0.8f, -0.7f),
            new Vector2(1.5f, -1.0f)

        };
        int CurrentCam;
        private static Camera camera;
        internal static float CameraFov { get; set; } = 45;

        protected override async void OnTick()
        {
            Vector3 pos = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, (CameraOffsets[CameraType].Key.X + offsets[CameraType].X),
                (CameraOffsets[CameraType].Key.Y + offsets[CameraType].Y),
                (CameraOffsets[CameraType].Key.Z));
            if (Game.IsControlPressed(0, Control.MoveLeftOnly))
            {
                Game.PlayerPed.Task.LookAt(GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 1.2f, .5f, .7f), 1100);
            }
            else if (Game.IsControlPressed(0, Control.MoveRightOnly))
            {
                Game.PlayerPed.Task.LookAt(GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, -1.2f, .5f, .7f), 1100);
            }
            else
            {
                Game.PlayerPed.Task.LookAt(GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, 0f, .5f, .7f), 1100);
            }
            Vector3 pointAt = GetOffsetFromEntityInWorldCoords(Game.PlayerPed.Handle, CameraOffsets[CameraType].Value.X, CameraOffsets[CameraType].Value.Y, CameraOffsets[CameraType].Value.Z);
            if (!DoesCamExist(CurrentCam))
            {
                CurrentCam = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
                camera = new Camera(CurrentCam)
                {
                    Position = pos,
                    FieldOfView = CameraFov
                };
                camera.PointAt(pointAt);
                RenderScriptCams(true, false, 0, false, false);
                camera.IsActive = true;
            }
            else
            {
                if (camera.Position != pos)
                {
                    await UpdateCamera(camera, pos, pointAt);
                }
            }
        }
        private async Task UpdateCamera(Camera oldCamera, Vector3 pos, Vector3 pointAt)
        {
            var newCam = CreateCam("DEFAULT_SCRIPTED_CAMERA", true);
            var newCamera = new Camera(newCam)
            {
                Position = pos,
                FieldOfView = CameraFov
            };
            newCamera.PointAt(pointAt);
            oldCamera.InterpTo(newCamera, 1000, true, true);
            while (oldCamera.IsInterpolating || !newCamera.IsActive)
            {
                SetEntityCollision(Game.PlayerPed.Handle, false, false);
                //Game.PlayerPed.IsInvincible = true;
                Game.PlayerPed.IsPositionFrozen = true;
                await BaseScript.Delay(0);
            }
            await BaseScript.Delay(50);
            oldCamera.Delete();
            CurrentCam = newCam;
            camera = newCamera;
        }
        protected override void OnStop()
        {
            camera.IsActive = false;
            RenderScriptCams(false, false, 0, false, false);
            DestroyCam(CurrentCam, false);
            CurrentCam = -1;
            camera.Delete();
            base.OnStop();
        }
    }
}
