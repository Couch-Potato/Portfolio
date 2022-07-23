using System;
using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    /// <summary>
    /// Money interface. To be used primarily by the money service. If you need to do anything regarding a player's money, use the Money service.
    /// </summary>
    [UserInterface("money")]
    public class Money : UserInterface
    {
        private bool _visible = false;
        private float _money = 0;
        private float _modifierAmt = 0;
        [Configuration("m_visible")]
        public bool Visible { get => _visible; }

        [Configuration("m_modifier")]
        public float ModifierAmt { get => _modifierAmt; }

        [Configuration("m_amount")]
        public float CashAmt { get=>_money; set {
                CashOperation(value - _money);
            } }

        private async void MoneyShowExpire()
        {
            await BaseScript.Delay(6000);
            _visible = false;
            _modifierAmt = 0;
            UpdateAsync();
        }

        public void ShowMoneyBriefly()
        {
            _visible = true;
            UpdateAsync();
            MoneyShowExpire();
        }

        public void CashOperation(float money)
        {
            _modifierAmt = money;
            _money += money;
            _visible = true;
            UpdateAsync();
            MoneyShowExpire();
        }
        /// <summary>
        /// Overrides the money set for the UI controller.
        /// </summary>
        /// <remarks>
        /// To be used exclusively by the Money service. Do not use anywhere else.
        /// </remarks>
        /// <param name="amt">The amount</param>
        internal void OverrideSetMoney(float amt)
        {
            _money = amt;
        }
    }
}
