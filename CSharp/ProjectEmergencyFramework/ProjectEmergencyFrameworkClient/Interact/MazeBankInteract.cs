using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("mazeBank", "E", "MAZE BANK")]
    public class MazeBankInteract : RadiusInteractable
    {
        public MazeBankInteract()
        {
            Radius = 15f;
        }
        public override async Task<bool> CanShow()
        {
            return await base.CanShow();
        }
        protected override void OnInteract()
        {
            Services.DialogService.ShowDialog(new Services.DialogHandler() { Icon = Utility.BaseIcons.MissingIcon, Name = "MAZE BANK", Prompt = new Services.DialogPrompt("How can I help you today?", new Services.DialogOption[] {
                new Services.DialogOption("OPEN ACCOUNT", async (Services.DialogHandler handle)=>{
                    Services.DialogService.HideAllDialogs();
                    var debitCardInfo = await Utility.QueryService.QueryConcrete<ProjectEmergencyFrameworkShared.Data.Model.DebitCard>("GENERATE_BANK_ACCOUNT", 0);
                    dynamic props = new ExpandoObject();
                    props.name = debitCardInfo.HolderFirstName + " " + debitCardInfo.HolderLastName;
                    props.number = debitCardInfo.CardId;
                    Services.InventoryService.AddItem(Services.EquipmentService.ConstructEquipable("Debit Card", Utility.BaseIcons.MissingIcon, props));
                   
                }),

            }) });
        }

    }
}
