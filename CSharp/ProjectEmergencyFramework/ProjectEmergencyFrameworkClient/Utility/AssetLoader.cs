using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class AssetLoader
    {
        public static async Task LoadAnimDict(string name)
        {
            if (!HasAnimDictLoaded(name))
            {
                RequestAnimDict(name);
                while (!HasAnimDictLoaded(name))
                {
                    await BaseScript.Delay(0);
                }
            }
        }

        public static async Task LoadModel(string name)
        {
            while (!HasModelLoaded((uint)GetHashKey(name)))
            {
                RequestModel((uint)GetHashKey(name));
                await BaseScript.Delay(1);
            }
        }
        public static async Task LoadDoorSystemPhysics(int hash)
        {
            if (!DoorSystemGetIsPhysicsLoaded(hash))
            {
                while (!DoorSystemGetIsPhysicsLoaded(hash))
                    await BaseScript.Delay(0);
            }
        }
    }
}
