using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("intercom", true)]
    public class ApartmentIntercom : UserInterface
    {
        [Configuration("config")]
        public List<ApartmentMemberItem> items { get; set; } = new List<ApartmentMemberItem>();

        [Reactive("apartment")]
        public int setApartment { get => 0; set
            {
                DoSetApartment(value);
            } 
        }

        public async void DoSetApartment(int apxid)
        {
            Apartment apt = await QueryService.QueryConcrete<Apartment>("GET_APARTMENT", items[apxid].apartmentId);
            UniverseService.TeleportToUniverse(apt.UniverseId);
            var unv = await UniverseService.GetUniverse(apt.UniverseId);
            var arch = ConfigurationService.GetArchetype<ProjectEmergencyFrameworkShared.Configuration.Schema.UniverseArchetype>(unv.UniverseType);
            Interact.InteractService.ConstructInteract("apartment@exit", ConfigurationService._loc_to_vector_3(arch.SpawnCoords), new {});
            Hide();
        }

        protected override async Task ConfigureAsync()
        {
            dynamic aptData = QueryService.QueryConcrete<dynamic>("GET_AVAIL_APARTMENTS", true);
            items.Clear();
            foreach (var apt in aptData)
            {
                items.Add(new ApartmentMemberItem()
                {
                    firstName = apt.firstName,
                    lastName = apt.lastName,
                    apartmentId = apt.apartmentId
                });
            }
            //await base.ConfigureAsync();
        }
    }
    public class ApartmentMemberItem
    {
        public string firstName { get; set; }
        public string apartmentId { get; set; }
        public string lastName { get; set; }
    }
}
