using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    
    [Equipable("Radio", "/assets/inventory/radio.svg")]
    public class RadioEquipable:Equipable
    {
        PETask radioTask;
        protected override void OnInstanced()
        {
            if (IsInDiscreteMode) return;
            Services.VoiceService.SetRadioFrequency("COUNTY");
            radioTask = TaskService.InvokeUntilExpire(async () =>
            {
                if (isPressed(57))
                {
                    DispatchService.PanicButton();
                }
                return false;
            });
        }
        protected override void OnDeInstanced()
        {
            Services.VoiceService.EndRadio();
            TaskService.ForceExpireTask(radioTask);
        }
        bool eqpd = false;
        
        private bool isPressed(int cid)
        {
            return IsControlJustPressed(0, cid) || IsDisabledControlJustPressed(0, cid);
        }
        PresetRadioFrequency RadioFrequency = PresetRadioFrequency.County;
        protected override void OnEquip()
        {
            eqpd = true;
            Services.VoiceService.RadioInterface.ShowRadio();
            Services.TaskService.InvokeUntilExpire(async () =>
            {
                
                //UP
                if (isPressed(27))
                {
                    var sel = Services.VoiceService.RadioInterface.Config.selected;
                    if (sel == 0)
                    {
                        sel = 2;
                    }else
                    {
                        sel --;
                    }
                    Services.VoiceService.RadioInterface.Config.selected = sel;
                    Services.VoiceService.RadioInterface.Update();
                }
                if (isPressed(173))
                {
                    var sel = Services.VoiceService.RadioInterface.Config.selected;
                    if (sel == 2)
                    {
                        sel = 0;
                    }
                    else
                    {
                        sel++;
                    }
                    Services.VoiceService.RadioInterface.Config.selected = sel;
                    Services.VoiceService.RadioInterface.Update();
                }
                if (isPressed(176))
                {
                    var sel = Services.VoiceService.RadioInterface.Config.selected;
                    if (sel == 0)
                    {
                        // CALL BACKUP
                        DispatchService.DemandBackup();
                    }
                    if (sel == 1)
                    {
                        // SWITCH COUNTY / METRO
                        if (RadioFrequency == PresetRadioFrequency.County)
                        {
                            RadioFrequency = PresetRadioFrequency.Metro;
                            VoiceService.SetRadioFrequency("METRO");
                            return !eqpd;
                        }
                        if (RadioFrequency == PresetRadioFrequency.Metro)
                        {
                            RadioFrequency = PresetRadioFrequency.County;
                            VoiceService.SetRadioFrequency("COUNTY");
                            return !eqpd;
                        }
                    }
                    if (sel == 2)
                    {
                        // CALL EMS
                        DispatchService.DemandEMS();
                    }
                }
                return !eqpd;
            });
        }
        protected override void OnUnEquip()
        {
            Services.VoiceService.RadioInterface.HideRadio();
            eqpd = false;
        }
    }
}
