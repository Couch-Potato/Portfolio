using CitizenFX.Core;
using static CitizenFX.Core.Native.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectEmergencyFrameworkClient.Services;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Utility;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    struct CamDef
    {
        Vector3 Face;
        Vector3 Body;
        float FaceH;
        float BodyH;
    }

    [UserInterface("characterbuilder", true)]
    public class CharacterCreator : UserInterface
    {

        ProjectEmergencyFrameworkShared.Data.Model.CharacterPhysicalData CharacterPhysicalData = new ProjectEmergencyFrameworkShared.Data.Model.CharacterPhysicalData()
        {
            BeardColorId = 0,
            BeardId = 0,
            BeardOpacity = 0,
            SunDamageOpacity = 0,
            Sex = 0,
            SunDamageStyleId = 0,
            AgeingStyleId = 0,
            AgeingStyleOpacity = 0,
            EyebrowColor = 0,
            EyebrowOpacity = 0,
            EyebrowStyleId = 0,
            HairColorId = 0,
            HairId = 0,
            HeadId = 0,
            MoleOpacity = 0,
            MoleStyleId = 0,
            Torso = 15,
            FaceFeaturesByIndex = new List<float>
            {
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
                .5f,
            }
        };
        ProjectEmergencyFrameworkShared.Data.Model.ClothingData ClothingData = new ProjectEmergencyFrameworkShared.Data.Model.ClothingData()
        {
            Shirt = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
            {
                Drawable = 252,
                Texture = 0,
            },
            Shoes = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
            {
                Drawable = 34,
                Texture = 0
            },
            Pants = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
            {
                Drawable=21,
                Texture = 0
            },
            Undershirt = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
            {
                Drawable = 15,
                Texture = 0
            }
        };


        int Face=0;
        int Eyes=0;
        int EyeColor=0;
        int Hair=2;
        int HairColor = 2;
        int Shirt = 252;
        int Pants = 21;
        int Shoes = 34;
        int ShirtUpper = 15;


        int _torso = 0;
        int _variant = 0;
        int _category = 0; // FACE, HAIR, SHIRTS, UNDERSHIRTS, PANTES SHOES
        int _drawable = 0;





        string FirstName = "";
        string LastName = "";
        string DOBMonth = "";
        string DOBYear = "";
        string DOBDay = "";

        public CharacterCreator() : base()
        {
           
            //Show();
        }
        public override void AfterShow()
        {
            
        }

        [Reactive("variant")]
        public int variant { get=>0; set
            {
                switch(_category)
                {
                    case 2:
                        _variant = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 11, _drawable, _variant, 0);
                        ClothingData.Shirt.Texture = value;
                        break;
                    case 3:
                        _variant = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 8, _drawable, _variant, 0);
                        ClothingData.Undershirt.Texture = value;
                        break;
                    case 4:
                        _variant = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 4, _drawable, _variant, 0);
                        ClothingData.Pants.Texture = value;
                        break;
                    case 5:
                        _variant = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 6, _drawable, _variant, 0);
                        ClothingData.Shoes.Texture = value;
                        break;
                }
            } 
        }

        [Reactive("torso")]
        public int torso
        {
            get => 0; set
            {
                CharacterPhysicalData.Torso = value;
                SetPedComponentVariation(Game.PlayerPed.Handle, 3, value, 0, 0);
            }
        }

        [Reactive("ff")]
        public string c_00
        {
            get => ""; set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                SetPedFaceFeature(Game.PlayerPed.Handle, pedData.id, float.Parse(pedData.scale)/100f);
                CharacterPhysicalData.FaceFeaturesByIndex[pedData.id] = float.Parse(pedData.scale) / 100f;
            }
        }

        [Reactive("drawable")]
        public int c1
        {
            get => 0; set
            {
                switch (_category)
                {
                    case 0:
                        CharacterPhysicalData.HeadId = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 0, value, 0, 0);
                        SetPedHeadBlendData(Game.PlayerPed.Handle, value, 0, value, value, 0, value, 0f, 0f, 1f, true);
                        break;
                    case 1:
                        CharacterPhysicalData.HairId = value;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 2, value, 0, 0);
                        break;
                    case 2:
                        _drawable = value;
                        _variant = 0;
                        SetPedComponentVariation(Game.PlayerPed.Handle, 11, _drawable, _variant, 0);
                        ClothingData.Shirt = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
                        {
                            Texture = 0,
                            Drawable = value
                        };
                        break;
                    case 3:
                        _drawable = value;
                        _variant = 0;

                        SetPedComponentVariation(Game.PlayerPed.Handle, 8, _drawable, _variant, 0);
                        ClothingData.Undershirt = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
                        {
                            Texture = 0,
                            Drawable = value
                        };
                        break;
                    case 4:
                        _drawable = value;
                        _variant = 0;

                        SetPedComponentVariation(Game.PlayerPed.Handle, 4, _drawable, _variant, 0);
                        ClothingData.Pants = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
                        {
                            Texture = 0,
                            Drawable = value
                        };
                        break;
                    case 5:
                        _drawable = value;
                        _variant = 0;

                        SetPedComponentVariation(Game.PlayerPed.Handle, 6, _drawable, _variant, 0);
                        ClothingData.Shoes = new ProjectEmergencyFrameworkShared.Data.Model.DrawableItem()
                        {
                            Texture = 0,
                            Drawable = value
                        };
                        break;
                }
            }
        }

        [Configuration("aux_config")]
        public obConfig aconfg { get; set; }

        [Configuration("clothing")]
        public CConfig config { get; set; }

        protected override async Task ConfigureAsync()
        {
            uint model = (uint)GetHashKey("mp_m_freemode_01");

            if (!HasModelLoaded(model))
            {
                RequestModel(model);
                while (!HasModelLoaded(model))
                {
                    await BaseScript.Delay(0);
                }
            }
            SetPlayerModel(PlayerId(), model);
            config = new CConfig();

            config.undershirts = new List<CConfigItem>();
            for (int i=0;i<GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle,8); i++)
            {
                config.undershirts.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 8, i)
                });
            }

            config.shoes = new List<CConfigItem>();
            for (int i = 0; i < GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 6); i++)
            {
                config.shoes.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 6, i)
                });
            }

            config.shirts = new List<CConfigItem>();
            for (int i = 0; i < GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 11); i++)
            {
                config.shirts.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 11, i)
                });
            }

            config.pants = new List<CConfigItem>();
            for (int i = 0; i < GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 4); i++)
            {
                config.pants.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 4, i)
                });
            }

            config.face = new List<CConfigItem>();
            for (int i = 0; i < GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 0); i++)
            {
                config.face.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 0, i)
                });
            }

            config.hair = new List<CConfigItem>();
            for (int i = 0; i < GetNumberOfPedDrawableVariations(Game.PlayerPed.Handle, 2); i++)
            {
                config.hair.Add(new CConfigItem()
                {
                    name = "",
                    icon = "",
                    variants = GetNumberOfPedTextureVariations(Game.PlayerPed.Handle, 2, i)
                });
            }

            aconfg = new obConfig()
            {
                ageStyleCount = GetNumHeadOverlayValues(3),
                beardStyleCount = GetNumHeadOverlayValues(1),
                eyebrowStyleCount = GetNumHeadOverlayValues(2),
                moleStyleCount = GetNumHeadOverlayValues(9),
                sunDamageStyleCount = GetNumHeadOverlayValues(7)
            };
            SetPedHeadBlendData(Game.PlayerPed.Handle, 0, 0, 0, 0, 0, 0, 0f, 0f, 1f, true);

        }

        [Reactive("category")]
        public int c2
        {
            get => 0; set
            {
                _drawable = 0;
                _variant = 0;
                _category = value;
                switch (value)
                {
                    case 0:
                        CameraService.GetCameraOperator<Services.Cams.ClothingCam>().CameraType = Services.Cams.ClothingCam.HEAD;
                        break;
                    case 1:
                        CameraService.GetCameraOperator<Services.Cams.ClothingCam>().CameraType = 1;
                        break;
                    case 2:
                    case 3:
                        CameraService.GetCameraOperator<Services.Cams.ClothingCam>().CameraType = 2;
                        break;
                    case 4:
                        CameraService.GetCameraOperator<Services.Cams.ClothingCam>().CameraType = 3;
                        break;
                    case 5:
                        CameraService.GetCameraOperator<Services.Cams.ClothingCam>().CameraType = 4;
                        break;
                }
            }
        }

        [Reactive("hair_color")]
        public int c3
        {
            get => 0; set
            {
                CharacterPhysicalData.HairColorId = value;
                SetPedHairColor(Game.PlayerPed.Handle, value, 0);
            }
        }

        [Reactive("beard_style")]
        public int c4
        {
            get => 0; set
            {
                CharacterPhysicalData.BeardId = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 1, value, CharacterPhysicalData.BeardOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, CharacterPhysicalData.BeardColorId, CharacterPhysicalData.BeardColorId);
            }
        }

        [Reactive("beard_opacity")]
        public string c5
        {
            get => null; set
            {
                CharacterPhysicalData.BeardOpacity = float.Parse(value) / 100f;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 1, CharacterPhysicalData.BeardId, CharacterPhysicalData.BeardOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, CharacterPhysicalData.BeardColorId, CharacterPhysicalData.BeardColorId);

            }
        }

        [Reactive("beard_color")]
        public int c6
        {
            get => 0; set
            {
                CharacterPhysicalData.BeardColorId = value;
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 1, 1, CharacterPhysicalData.BeardColorId, CharacterPhysicalData.BeardColorId);
            }
        }

        [Reactive("ageing_style")]
        public int c7
        {
            get => 0; set
            {
                CharacterPhysicalData.AgeingStyleId = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 3, CharacterPhysicalData.AgeingStyleId, CharacterPhysicalData.AgeingStyleOpacity);
            }
        }

        [Reactive("ageing_opacity")]
        public string c8
        {
            get => null; set
            {
                CharacterPhysicalData.AgeingStyleOpacity = float.Parse(value) / 100f;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 3, CharacterPhysicalData.AgeingStyleId, CharacterPhysicalData.AgeingStyleOpacity);

            }
        }

        [Reactive("sun_damage_style")]
        public int c9
        {
            get => 0; set
            {
                CharacterPhysicalData.SunDamageStyleId = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 7, CharacterPhysicalData.SunDamageStyleId, CharacterPhysicalData.SunDamageOpacity);
            }
        }

        [Reactive("sun_damage_opacity")]
        public string c10
        {
            get => null; set
            {
                CharacterPhysicalData.SunDamageOpacity = float.Parse(value) / 100f;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 7, CharacterPhysicalData.SunDamageStyleId, CharacterPhysicalData.SunDamageOpacity);
            }
        }

        [Reactive("mole_style")]
        public int c11
        {
            get => 0; set
            {
                CharacterPhysicalData.MoleStyleId = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 9, CharacterPhysicalData.MoleStyleId, CharacterPhysicalData.MoleOpacity);
            }
        }

        [Reactive("mole_opacity")]
        public string c12
        {
            get => null; set
            {
                CharacterPhysicalData.MoleOpacity = float.Parse(value) / 100f;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 9, CharacterPhysicalData.MoleStyleId, CharacterPhysicalData.MoleOpacity);
            }
        }

        [Reactive("eyebrow_style")]
        public int c13
        {
            get => 0; set
            {
                CharacterPhysicalData.EyebrowStyleId = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 2, CharacterPhysicalData.EyebrowStyleId, CharacterPhysicalData.EyebrowOpacity);
            }
        }

        [Reactive("eyebrow_opacity")]
        public string c14
        {
            get => null; set
            {
                CharacterPhysicalData.EyebrowOpacity = float.Parse(value) / 100f;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 2, CharacterPhysicalData.EyebrowStyleId, CharacterPhysicalData.EyebrowOpacity);
            }
        }

        [Reactive("eyebrow_color")]
        public int c15
        {
            get => 0; set
            {
                CharacterPhysicalData.EyebrowColor = value;
                SetPedHeadOverlay(Game.PlayerPed.Handle, 2, CharacterPhysicalData.EyebrowStyleId, CharacterPhysicalData.EyebrowOpacity);
                SetPedHeadOverlayColor(Game.PlayerPed.Handle, 2, 1, CharacterPhysicalData.EyebrowColor, CharacterPhysicalData.EyebrowColor);
            }
        }
        bool isCharacterCreate = false;
        [Reactive("create")]
        public async void Final()
        {
            if (isCharacterCreate) return;
            try
            {
                isCharacterCreate = true;
                InterfaceController.HideInterface("characterbuilder");
                DoScreenFadeOut(500);
                CameraService.Terminate();
                string dmvPhoto = await HeadshotService.GetHeadshotOfPed(Game.PlayerPed.Handle);
                var day = int.Parse(Properties.dd);
                var month = int.Parse(Properties.mm);
                var year = int.Parse(Properties.yy);
                CharacterPhysicalData.Sex = Properties.sex == "MALE" ? 0 : 1;
                var cname = await CharacterService.CreateCharacter(new ProjectEmergencyFrameworkShared.Data.Model.CharacterInit()
                {
                    FirstName = Properties.firstName,
                    LastName = Properties.lastName,
                    DOB = new ProjectEmergencyFrameworkShared.Data.Model.DateOfBirth()
                    {
                        Day = day,
                        Month = month,
                        Year = year
                    },
                    Clothing = ClothingData,
                    Physical = CharacterPhysicalData,
                    DMVPhoto = dmvPhoto
                });

                ProjectEmergencyFrameworkShared.Configuration.Schema.ApartmentConfig apt = ConfigurationService.CurrentConfiguration.Apartments[Properties.aptId];

                await QueryService.QueryConcrete<bool>("CREATE_APARTMENT", new
                {
                    config = apt.Id,
                    universe = apt.UniverseId,
                    owner = cname
                });
                CharacterService.SetCharacterAndSpawn(cname);
            }
            catch
            {

            }
        }
        protected override async Task BeforeShow()
        {
            FreezeEntityPosition(GetPlayerPed(-1), false);
            DisableAllControlActions(0);
            DisplayRadar(false);
            SetEntityVisible(Game.PlayerPed.Handle, true, true);
            var playerPed = PlayerPedId();
            int camera = 0;

            DoScreenFadeOut(500);

            if (!HasAnimDictLoaded("mp_character_creation@customise@male_a"))
            {
                while (!HasAnimDictLoaded("mp_character_creation@customise@male_a"))
                {
                    RequestAnimDict("mp_character_creation@customise@male_a");
                    await BaseScript.Delay(10);
                }
            }

          

            uint model = (uint)GetHashKey("mp_m_freemode_01");

            if (!HasModelLoaded(model))
            {
                RequestModel(model);
                while (!HasModelLoaded(model))
                {
                    await BaseScript.Delay(0);
                }
            }

            await BaseScript.Delay(2000);

            SetPlayerModel(PlayerId(), model);

            SetPedDefaultComponentVariation(Game.PlayerPed.Handle);

            // DEFAULT CONFIG

            SetPedComponentVariation(Game.PlayerPed.Handle, 4, 21, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 2, 2, 2, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 8, 15, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 3, 15, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 11, 252, 0, 0);

            SetPedComponentVariation(Game.PlayerPed.Handle, 6, 34, 0, 0);

            await BaseScript.Delay(2000);
            DestroyAllCams(true);

            camera = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 403.52f, -1000.72f, -99.01f, 0.00f, 0.00f, 0.00f, 30.00f, false, 0);
            SetCamActive(camera, true);
            RenderScriptCams(true, false, 2000, true, true);



            await BaseScript.Delay(500);
            DoScreenFadeIn(2000);

            SetEntityCoords(Game.PlayerPed.Handle, 405.59f, -997.18f, -99.0f, false, false, false, true);
            SetEntityHeading(Game.PlayerPed.Handle, 90.00f);

            await BaseScript.Delay(500);

            var cam3 = CreateCamWithParams("DEFAULT_SCRIPTED_CAMERA", 402.99f, -998.02f, -99.00f, 0.00f, 0.00f, 0.00f, 50.00f, false, 0);
            PointCamAtCoord(cam3, 402.99f, -998.02f, -99.00f);
            SetCamActiveWithInterp(camera, cam3, 5000, 1, 1);
            TaskPlayAnim(Game.PlayerPed.Handle, "mp_character_creation@customise@male_a", "intro", 1.0f, 1.0f, 4000, 0, 1, false, false, false);
            await BaseScript.Delay(5000);


            var coords = GetEntityCoords(Game.PlayerPed.Handle, true);
            if (GetDistanceBetweenCoords(coords.X, coords.Y, coords.Z, 402.89f, -996.87f, -99.0f, true) > 0.5) {
                SetEntityCoords(Game.PlayerPed.Handle, 402.89f, -996.87f, -99.0f, false, false, false, true);

                SetEntityHeading(Game.PlayerPed.Handle, 173.97f);
            }
            await BaseScript.Delay(500);
            FreezeEntityPosition(GetPlayerPed(-1), true);

            CameraService.SetCamera<Services.Cams.ClothingCam>();
           
        }
    }
    public class CConfig
    {
        public List<CConfigItem> shirts { get; set; }
        public List<CConfigItem> pants { get; set; }
        public List<CConfigItem> face { get; set; }
        public List<CConfigItem> hair { get; set; }
        public List<CConfigItem> undershirts { get; set; }
        public List<CConfigItem> shoes { get; set; }


    }
    public class obConfig
    {
        public int beardStyleCount { get; set; }
        public int ageStyleCount { get; set; }
        public int sunDamageStyleCount { get; set; }
        public int moleStyleCount { get; set; }
        public int eyebrowStyleCount { get; set; }

    }
    public class CConfigItem
    {
        public string name { get; set; }
        public int variants { get; set; }
        public string icon { get; set; }
    }
    public class PedFaceFeatureData
    {
        public int id { get; set; }
        public string scale { get; set; }
    }
}
