using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkShared.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("hotbar", false)]
    public class Hotbar : UserInterface
    {
        public Hotbar()
        {

        }

        [Configuration("hotbarItems")]
        public List<InventoryItem> Items
        {
            get => InventoryService.InventoryItems;
        }

        private bool _showing = false;
        private int _selected = -1;
    
        [Configuration("showHotbar")]
        public bool Showing { get => _showing; set => _showing = value; }

        [Configuration("selected")]
        public int Selected { get => _selected; set => _selected = value; }

        public void ShowPopup()
        {
            Showing = true;
            Update();
        }

        public void HidePopup()
        {
            Showing = false;
            Update();
        }
        public override void AfterShow()
        {
            CitizenFX.Core.Native.API.SetNuiFocus(false, false);
            base.AfterShow();
        }



    }
}
