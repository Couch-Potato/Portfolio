using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Effects
{
    public class TimedHealthEffect : IHealthEffect
    {
        public uint TimeStart { get; private set; }
        public float EffectDuration { get; set; } = 0f;
        public void OnEffectStart()
        {
            TimeStart = Timestamp();
        }

        public void OnEffectStop()
        {
            
        }
        public static uint Timestamp()
        {
            return (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
        public void Tick()
        {
            if (EffectDuration - (TimeStart - Timestamp()) <= 0)
            {
                //HealthEffectService.StopHealthEffect(this);
            }
        }
    }
}
