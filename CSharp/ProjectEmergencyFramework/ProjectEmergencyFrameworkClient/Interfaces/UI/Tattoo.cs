using CitizenFX.Core;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Services.Cams;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("tattoo", true)]
    public class Tattoo : UserInterface
    {
        CharacterSnapshot snapshot;

        [Configuration("config")]
        public List<int> tattooMaxxes { get; set; } = new List<int>()
        {
            1, // Head
            1, //Torso
            1, // Left Leg
            1, // Right Leg
            1, // Right Arm
            1, // Left Arm
        };

        public Dictionary<int, List<Tuple<string, string>>> tattooDict = new Dictionary<int, List<Tuple<string, string>>>();



        private int _t_s = 0;
        private int _t_type = 0;

        [Reactive("tattoo")]
        public int tattooSelected { get => _t_s; set
            {

                snapshot.RestoreTattoo();
                var tattoo = tattooDict[_t_type][value];
                _t_s = value;
                SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Item1), (uint)GetHashKey(tattoo.Item2));

            } 
        }

        [Reactive("bodypart")]
        public int bodyPart { get => _t_type; set
            {
                //tattooSelected = 0;
                _t_type = value;
                tattooSelected = 0;
                doCameraAndClothing(value);
            } 
        }
        private void doCameraAndClothing(int id)
        {
            if (id == 0)
            {
                snapshot.RestoreClothes();
                CameraService.GetCameraOperator<ClothingCam>().CameraType = ClothingCam.HEAD;
            }if (id == 1 || id== 4 || id == 5)
            {
                snapshot.RestoreClothes();
                SetPedComponentVariation(Game.PlayerPed.Handle, 8, 15, 0, 0);

                SetPedComponentVariation(Game.PlayerPed.Handle, 3, 15, 0, 0);

                SetPedComponentVariation(Game.PlayerPed.Handle, 11, 252, 0, 0);
                CameraService.GetCameraOperator<ClothingCam>().CameraType = ClothingCam.FULL_ARMS;
            }
            if (id == 2 || id == 3)
            {
                snapshot.RestoreClothes();
                SetPedComponentVariation(Game.PlayerPed.Handle, 4, 21, 0, 0);
                CameraService.GetCameraOperator<ClothingCam>().CameraType = ClothingCam.LOWER;
            }
        }

        [Reactive("purchase")]
        public async void Purchase()
        {
            // Do purchase logic here
            float price = (((_t_s - (_t_s % 10)) / 10) * 400) + 400;
            var tattoo = tattooDict[_t_type][_t_s];
            //SetPedDecoration(Game.PlayerPed.Handle, (uint)GetHashKey(tattoo.Item1), (uint)GetHashKey(tattoo.Item2));
            if (!await Services.TransactionService.Pay(price, "TATTOO - " + tattoo.Item2.ToUpper()))
                return;

            if (CharacterService.CurrentCharacter.Physical.Tattoos == null)
                CharacterService.CurrentCharacter.Physical.Tattoos = new List<ProjectEmergencyFrameworkShared.Data.Model.TattooItem>();
            CharacterService.CurrentCharacter.Physical.Tattoos.Add(new ProjectEmergencyFrameworkShared.Data.Model.TattooItem()
            {
                Name=tattoo.Item2,
                CollectionName=tattoo.Item1,
                ZoneName=((TattooZoneData)GetPedDecorationZoneFromHashes((uint)GetHashKey(tattoo.Item1), (uint)GetHashKey(tattoo.Item2))).ToString()
            });
            snapshot.character.Physical = CharacterService.CurrentCharacter.Physical;
            QueryService.QueryConcrete<bool>("UPDATE_PHYSICAL", CharacterService.CurrentCharacter.Physical);
        }

        protected override Task ConfigureAsync()
        {
            snapshot = CharacterService.GetCharacterSnapshot();

            foreach (var tattoo in ClothingData.Tattoos)
            {
                if (tattoo.zoneName == "ZONE_TORSO")
                    _addToTattooDict(1, tattoo.collectionName, tattoo.name);
                if (tattoo.zoneName == "ZONE_HEAD")
                    _addToTattooDict(0, tattoo.collectionName, tattoo.name);
                if (tattoo.zoneName == "ZONE_LEFT_ARM")
                    _addToTattooDict(5, tattoo.collectionName, tattoo.name);
                if (tattoo.zoneName == "ZONE_LEFT_LEG")
                    _addToTattooDict(2, tattoo.collectionName, tattoo.name);
                if (tattoo.zoneName == "ZONE_RIGHT_ARM")
                    _addToTattooDict(4, tattoo.collectionName, tattoo.name);
                if (tattoo.zoneName == "ZONE_RIGHT_LEG")
                    _addToTattooDict(3, tattoo.collectionName, tattoo.name);
            }

            tattooMaxxes = new List<int>()
            {
                tattooDict[0].Count,
                tattooDict[1].Count,
                tattooDict[2].Count,
                tattooDict[3].Count,
                tattooDict[4].Count,
                tattooDict[5].Count
            };
            return base.ConfigureAsync();
        }
        private void _addToTattooDict(int type, string collection, string name)
        {
            if (!tattooDict.ContainsKey(type))
                tattooDict.Add(type, new List<Tuple<string, string>>());
            tattooDict[type].Add(new Tuple<string, string>(collection, name));
        }
        protected override Task BeforeShow()
        {
            CameraService.SetCamera<ClothingCam>();
            return base.BeforeShow();
        }
        protected override void Cleanup()
        {
            CameraService.Terminate();
            snapshot.Restore();
            base.Cleanup();
        }
    }
}
