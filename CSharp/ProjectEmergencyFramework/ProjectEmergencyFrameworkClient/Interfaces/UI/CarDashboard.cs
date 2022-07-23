using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("carDashboard", false)]
    public class CarDashboard:UserInterface
    {
        private DashboardConfiguration configuration = new DashboardConfiguration()
        {
            mph=0,
            rpm=0,
            fuelPercent=0,
            maxSpeed = 1,
            gear="D"
        };

        [Configuration("carData")]
        public DashboardConfiguration Configuration { get => configuration;set {
                configuration = value;
                Update();
            } 
        }

        public override void AfterShow()
        {
            PLDService.TogglePLDLocation(false);
            base.AfterShow();
        }
        protected override void Cleanup()
        {
            PLDService.TogglePLDLocation(true);
            base.Cleanup();
        }

    }
    public class DashboardConfiguration
    {
        public int mph { get; set; }
        public float rpm { get; set; }
        public float maxSpeed { get; set; }
        public float fuelPercent { get; set; }
        public string gear { get; set; }
    }
}
