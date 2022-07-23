using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Effects
{
    public interface IHealthEffect
    {
        void Tick();
        void OnEffectStart();
        void OnEffectStop();
    }
}
