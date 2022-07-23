using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interact
{
    [Interactable("cj@bail", "E", "POST BAIL", true)]
    public class PayBailInteract : RadiusInteractable
    {
        public PayBailInteract()
        {
            Radius = 1.5f;
        }
        public override Task<bool> CanShow()
        {
            return base.CanShow();
        }
        protected override async void OnInteract()
        {
            bool success =  await TransactionService.Pay(Properties.bailAmount,"BAIL - 2022CR:" + Properties.arrestId);
            if (success)
            {
                await QueryService.QueryConcrete<bool>("POST_BAIL", CharacterService.CurrentCharacter.Id);
                CharacterService.SetCharacterAndSpawn(CharacterService.CurrentCharacter.Id);
            }
        }
    }

    [Interactable("cj@citation", "G", "PAY FINES", true)]
    public class PayCitationInteract : RadiusInteractable
    {
        public PayCitationInteract()
        {
            Radius = 1.5f;
        }
        public override Task<bool> CanShow()
        {
            return base.CanShow();
        }
        protected override async void OnInteract()
        {
            float fineAmt = await QueryService.QueryConcrete<float>("GET_FINES", CharacterService.CurrentCharacter.Id);

            bool success = await TransactionService.Pay(fineAmt, "FINES - OWED TO STATE");
            if (success)
            {
                await QueryService.QueryConcrete<bool>("POST_FINES", CharacterService.CurrentCharacter.Id);
                HUDService.ShowHelpText($"Fines paid! [${fineAmt.ToString()}]", "none", 5f);
            }
        }
    }
}
