using ProjectEmergencyFrameworkClient.Interfaces.UI;
using ProjectEmergencyFrameworkClient.Services;
using ProjectEmergencyFrameworkClient.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Effects
{
    public class HealthEffectService
    {
        static HealthEffectInterface healthEffectInterface = new HealthEffectInterface();
        static List<DiscoveredItem> discoveredEffects;

        public static List<IHealthEffect> ActivatedEffects = new List<IHealthEffect>();

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void InitHealthEffects()
        {
            discoveredEffects = ClassDiscovery.DiscoverWithAttribute<HealthEffectAttribute>();
            DebugService.DebugCall("HEALTH", "FOUND: " + discoveredEffects.Count);
            healthEffectInterface.Show();
        }
        [ExecuteAt(ExecutionStage.Tick)]
        public static void EffectTick()
        {
            foreach (var effect in ActivatedEffects)
            {
                effect.Tick();
            }
        }
        public static void StopHealthEffect(string name)
        {
            List<IHealthEffect> expired = new List<IHealthEffect>();
            foreach (var effect in ActivatedEffects)
            {
                if (effect.GetType().GetCustomAttribute<HealthEffectAttribute>().Name == name)
                {
                    expired.Add(effect);
                }
            }
            foreach (var effect in expired)
            {
                StopHealthEffect(effect);
            }
        }
        public static void StopHealthEffect(IHealthEffect effect)
        {
            effect.OnEffectStop();
            ActivatedEffects.Remove(effect);
            var attb = effect.GetType().GetCustomAttribute<HealthEffectAttribute>();
            healthEffectInterface.RemoveEffect(attb.Caption, attb.Icon, attb.Color);
        }
        public static void StopAllHealthEffects()
        {
            foreach (var healthEffect in ActivatedEffects)
            {
                var attb = healthEffect.GetType().GetCustomAttribute<HealthEffectAttribute>();
                healthEffectInterface.RemoveEffect(attb.Caption, attb.Icon, attb.Color);
                healthEffect.OnEffectStop();
            }
            ActivatedEffects.Clear();
        }
        public static bool IsEffectRunning(string name)
        {
            foreach (var effect in ActivatedEffects)
            {
                if (effect.GetType().GetCustomAttribute<HealthEffectAttribute>().Name == name)
                {
                    return true;
                }
            }
            return false;
        }
        public static void StartHealthEffect(string name)
        {
            foreach (var effect in discoveredEffects)
            {
                if (effect.GetAttribute<HealthEffectAttribute>().Name == name)
                {
                    IHealthEffect eff = effect.ConstructAs<IHealthEffect>();

                    eff.OnEffectStart();
                    ActivatedEffects.Add(eff);
                    var attb = effect.GetAttribute<HealthEffectAttribute>();
                    healthEffectInterface.AddEffect(attb.Caption, attb.Icon, attb.Color);
                }
                    
            }
        }
    }
}
