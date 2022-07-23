using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    [Equipable("Debit Card", "/assets/inventory/debitcard.svg", false)]
    public class DebitCardEquipable : Equipable
    {
        protected override void OnEquip()
        {
            base.OnEquip();
        }
        protected override void OnUnEquip()
        {
            base.OnUnEquip();
        }
    }
}
