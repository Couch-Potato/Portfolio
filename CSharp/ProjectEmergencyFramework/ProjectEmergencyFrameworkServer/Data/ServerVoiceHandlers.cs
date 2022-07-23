using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public static class ServerVoiceHandlers
    {
        public static FrequencyManager FrequencyManager = new FrequencyManager();
        public static PlayerFrequencyManager PFM = new PlayerFrequencyManager();
        [Queryable("V_JOIN_FREQ")]
        public static void JoinFreq(Query q, object i, Player px)
        {
            string freq = (string)i;
            JoinPlayerToFrequency(px, freq);
        }
        [Queryable("V_END_FREQ")]
        public static void EndFreq(Query q, object i, Player px)
        {
            RemovePlayerFromFrequency(px);
        }


        public static void JoinPlayerToFrequency(Player px, string freq)
        {
            if (PFM.IsPlayerInFrequency(px))
            {
                RemovePlayerFromFrequency(px, false);
            }
            PFM.SetFreq(px, freq);
            ClientQueryService.Instance.Exported["pma-voice"].setPlayerRadio(px.Handle, FrequencyManager[freq]);
        }
        public static void RemovePlayerFromFrequency(Player px, bool doPMARemove = true)
        {
            if (PFM.IsPlayerInFrequency(px))
            {
                FrequencyManager.TerminateMemberInFrequency(PFM.RemoveFreq(px));
                if (doPMARemove)
                    ClientQueryService.Instance.Exported["pma-voice"].setPlayerRadio(px.Handle, 0);
            }
            
        }
    }
    public class FrequencyItem
    {
        public string Name { get; set; }
        public int Members { get; set; } = 0;
    }
    public class PlayerFrequencyManager : Dictionary<Player, string>
    {
        public bool IsPlayerInFrequency(Player p)
        {
            return ContainsKey(p);
        }
        public void SetFreq(Player p, string s)
        {
            if (!ContainsKey(p))
            {
                Add(p, s);
            }else
            {
                this[p] = s;
            }
        }
        public string RemoveFreq(Player p)
        {
            string plyrfreq = this[p];
            Remove(p);
            return plyrfreq;
        }
    }
    public class FrequencyManager : List<FrequencyItem>
    {
        public int this[string key]
        {
            get
            {
                int i = 0;
                foreach (var freq in this)
                {
                    if (freq.Name == key)
                    {
                        freq.Members+=1;
                        return i + 1;
                    }
                    if (freq.Members == 0) {
                        freq.Members = 1;
                        freq.Name = key;
                        return i + 1;
                    }
                    i++;
                }
                this.Add(new FrequencyItem
                {
                    Name=key
                });
                return this.Count;
            }
        }
        public void TerminateMemberInFrequency(string key)
        {
            foreach (var freq in this)
            {
                if (freq.Name == key)
                {
                    freq.Members -= 1;
                }
            }
        }
    }
}
