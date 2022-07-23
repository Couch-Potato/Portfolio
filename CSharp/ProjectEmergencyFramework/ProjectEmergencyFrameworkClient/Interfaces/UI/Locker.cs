using CitizenFX.Core;
using Newtonsoft.Json;
using ProjectEmergencyFrameworkClient.Services.Cams;
using ProjectEmergencyFrameworkShared.Configuration.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{

    [UserInterface("locker", true)]
    public class Locker : UserInterface
    {
        [Configuration("uniforms")]
        public List<UniformConfigItem> uniforms { get; set; } = new List<UniformConfigItem>();

        [Configuration("agencies")]
        public List<OrganizationItem> agencies { get; set; } = new List<OrganizationItem>();

        protected override Task BeforeShow()
        {
            Services.CameraService.SetCamera<ClothingCam>();
            Services.CameraService.GetCameraOperator<ClothingCam>().CameraType = ClothingCam.FULL_BODY;
            return base.BeforeShow();
        }

        public List<Action<int>> overrideSet = new List<Action<int>>()
        {
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 11, i, 0, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 11); SetPedComponentVariation(Game.PlayerPed.Handle, 11, txt,i, 0); },
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 4, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 4); SetPedComponentVariation(Game.PlayerPed.Handle, 4, txt,i, 0); },
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 6, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 6); SetPedComponentVariation(Game.PlayerPed.Handle, 6, txt,i, 0); },

            (int i)=>SetPedPropIndex(Game.PlayerPed.Handle, 0, i, 0, true),
            (int i)=>{int txt = GetPedPropIndex(Game.PlayerPed.Handle, 0); SetPedPropIndex(Game.PlayerPed.Handle, 0, txt, i, true); },

            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 8, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 8); SetPedComponentVariation(Game.PlayerPed.Handle, 8, txt,i, 0); },
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 9, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 9); SetPedComponentVariation(Game.PlayerPed.Handle, 9, txt,i, 0); },
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 7, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 7); SetPedComponentVariation(Game.PlayerPed.Handle, 7, txt,i, 0); },
            (int i)=>{SetPedComponentVariation(Game.PlayerPed.Handle, 10, 0, i, 0); },
            (int i)=>{int txt = GetPedDrawableVariation(Game.PlayerPed.Handle, 10); SetPedComponentVariation(Game.PlayerPed.Handle, 10, txt,i, 0); },
        };


        [Reactive("uniform")]
        public int uniform
        {
            get => 0;
            set
            {
                var uniform = loadouts[value];
                setComponent(4, uniform.Pants);
                setComponent(11, uniform.Shirt);
                setComponent(6, uniform.Shoes);
                setProp(0, uniform.Hat);
                setComponent(8, uniform.Undershirt);
                setComponent(7, uniform.Accessory);
                setComponent(10, uniform.Badge);
                setComponent(9, uniform.BodyArmor);
                setComponent(5, uniform.BagsAndParachutes);
                setComponent(1, uniform.Mask);
                SetPedComponentVariation(Game.PlayerPed.Handle, 3, uniform.Torso, 0, 0);
            }
        }

        [Reactive("custom_clothes")]
        public string custom
        {
            get => null;
            set
            {
                var pedData = JsonConvert.DeserializeObject<PedFaceFeatureData>(value);
                overrideSet[pedData.id](int.Parse(pedData.scale));
            }
        }

        void setComponent(int id, ProjectEmergencyFrameworkShared.Configuration.Schema.UniformItem itm)
        {
            SetPedComponentVariation(Game.PlayerPed.Handle, id, itm.Drawable, itm.Texture, 0);
        }
        void setProp(int id, ProjectEmergencyFrameworkShared.Configuration.Schema.UniformItem itm)
        {
            //SetPedComponentVariation(Game.PlayerPed.Handle, id, itm.Drawable, itm.Texture, 0);
            SetPedPropIndex(Game.PlayerPed.Handle, id, itm.Drawable, itm.Texture, true);
        }


        public List<UniformLoadout> loadouts = new List<UniformLoadout>();
        protected override Task ConfigureAsync()
        {
            string orgcall = Properties.organization;
            foreach (var uniform in Services.ConfigurationService.CurrentConfiguration.Uniforms)
            {
                if (uniform.OrganizationId == orgcall)
                {
                    foreach (var loadout in uniform.Uniform)
                    {
                        loadouts.Add(loadout);
                        string nm = loadout.Name.Replace("Male ", "");
                        string nmx = nm.Split(' ')[0];
                        string nmz = nm.Replace(nmx + " ", "");
                        string leftOver = nmz.Split(' ')[0];
                        string name = "PATROL";
                        string subName = "";
                        if (leftOver.Length > 2)
                        {
                            for (int i = 0; i<2; i++)
                            {
                                name += leftOver[i] + " "; 
                            }
                        }
                        string dx = nmz.Replace(name, "");
                        foreach (var s in dx.Split(' '))
                        {
                            subName += s + " ";
                        }
                        subName = subName.TrimEnd(' ');
                        uniforms.Add(new UniformConfigItem()
                        {
                            agency=nmx,
                            name=name.TrimEnd(' '),
                            subName = subName
                        });
                    }
                }
            }
            if (orgcall == "BASE::SASP")
            {
                agencies = new List<OrganizationItem>()
                {
                    new OrganizationItem()
                    {
                        agency="LSPD",
                        icon="https://static.wikia.nocookie.net/gtawiki/images/7/7f/LSPD_logo_GTA_V.png/revision/latest?cb=20150425201508"
                    },
                    new OrganizationItem()
                    {
                        agency="BCSO",
                        icon="https://static.wikia.nocookie.net/gtawiki/images/3/32/LSSD.png/revision/latest?cb=20150829032550"
                    },
                    new OrganizationItem()
                    {
                        agency="SASP",
                        icon="https://static.wikia.nocookie.net/vectorrp/images/f/fc/SASP-SEALS.png/revision/latest/scale-to-width-down/1200?cb=20210814040803&path-prefix=id"
                    }
                };
            }
            if (orgcall == "BASE::LSFD")
            {
                agencies = new List<OrganizationItem>()
                {
                    new OrganizationItem()
                    {
                        agency="SanFire",
                        icon=""
                    },
                    new OrganizationItem()
                    {
                        agency="LSFD",
                        icon=""
                    },
                    new OrganizationItem()
                    {
                        agency="BCFD",
                        icon=""
                    }
                };
            }

            return base.ConfigureAsync();
        }
        protected override void Cleanup()
        {
            Services.CameraService.Terminate();
            base.Cleanup();
        }
    }
    public class UniformConfigItem
    {
        public string name { get; set; }
        public string subName { get; set; }
        public string agency { get; set; }
    }
    public class OrganizationItem
    {
        public string agency { get; set; }
        public string icon { get; set; }
    }
}
