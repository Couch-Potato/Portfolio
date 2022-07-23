using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("money@give", "E", "GIVE MONEY", true)]
    public class MoneyGiveInteract:RadiusInteractable
    {
        public MoneyGiveInteract()
        {
            Radius = 1.5f;
        }

        protected override void OnInteract()
        {
            RPC.RPCService.RemoteCall(NetworkGetNetworkIdFromEntity(Entity.Handle), "ADD_CASH_PLAYER", Properties.cashValue);
        }
    }
}
