using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
   
    [UserInterface("pleaBargain", true)]
    public class PleaAgreement : UserInterface
    {
        public static uint Timestamp()
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        private PleaConfiguration _cfg = new PleaConfiguration();
        [Configuration("pleaConfg")]
        public PleaConfiguration config { get=>_cfg; set=>_cfg=value; }
        protected override async Task ConfigureAsync()
        {
            string caseId = Properties.arrestId;
            ProjectEmergencyFrameworkShared.Data.Model.IncarcerationRecord record = await QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.IncarcerationRecord>("GET_INCARCERATION", caseId);
            ProjectEmergencyFrameworkShared.Data.Model.Arrests arrest = await QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.Arrests>("GET_ARREST", caseId);

            DebugService.Watchpoint("ARRESTING", new
            {
                
                arrest = arrest
            });

            config = new PleaConfiguration()
            {
                time = (record.TimeOfRelease - Timestamp()) / (60*5),
                fine = record.Fine,
                bail = record.Bail,
                charges = new List<PleaCharge>(),
                arrestId=caseId,
            };

            foreach (var charge in arrest.Charges)
            {
                config.charges.Add(new PleaCharge()
                {
                    charge = charge.Offense,
                    type = charge.Category
                });
            }
            await base.ConfigureAsync();
            //await base.ConfigureAsync();
        }
    }
    public class PleaConfiguration
    {
        public List<PleaCharge> charges { get; set; }
        public uint time { get; set; }
        public float bail { get; set; }
        public float fine { get; set; }
        public string arrestId { get; set; }
    }
    public class PleaCharge
    {
        public string charge { get; set; }
        public string type { get; set; }
    }
}
