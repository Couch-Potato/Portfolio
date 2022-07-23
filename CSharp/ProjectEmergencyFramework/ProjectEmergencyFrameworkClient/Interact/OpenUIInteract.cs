using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("ui@open@shop", "E", "ENTER SHOP")]
    public class OpenUIInteract : RadiusInteractable
    {
        protected override void OnInteract()
        {
            Interfaces.InterfaceController.ShowInterface(Properties.ui, Properties.props);
        }
        public OpenUIInteract()
        {
            Radius = 10f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
    }
}
