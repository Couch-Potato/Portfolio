using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class ClothingData
    {
        public static Dictionary<int, Dictionary<int, string>> Accessories = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Hair_Male = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Pants = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Shoes = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Tops = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Torsos = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Undershirts = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Masks = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Bracelets = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Ears = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Glasses = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Hats = new Dictionary<int, Dictionary<int, string>>();
        public static Dictionary<int, Dictionary<int, string>> Watches = new Dictionary<int, Dictionary<int, string>>();
        public static List<TattooEntry> Tattoos = new List<TattooEntry>();
        public static Dictionary<string, List<CrossRefEntry>> ClothingCrossRefs = new Dictionary<string, List<CrossRefEntry>>();

        public static Dictionary<int, int> BestTorsoForTop = new Dictionary<int, int>();

        public static Dictionary<int, Dictionary<int, string>> LoadClothingDatafile(string json)
        {
            var data = new Dictionary<int, Dictionary<int, string>>();
            string dataFile = LoadResourceFile("project_emergency_framework", json);
            var dataSerial = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, _CLOTHING_NAME_ENTRY>>>(dataFile);
            foreach (var kv_drawable in dataSerial)
            {
                var drData = new Dictionary<int, string>();
                foreach (var kv_text in kv_drawable.Value)
                {
                    drData.Add(int.Parse(kv_text.Key), kv_text.Value.Localized);
                    if (!ClothingCrossRefs.ContainsKey(kv_text.Value.Localized))
                    {
                        ClothingCrossRefs.Add(kv_text.Value.Localized, new List<CrossRefEntry>());
                    }
                    ClothingCrossRefs[kv_text.Value.Localized].Add(new CrossRefEntry()
                    {
                        Drawable = int.Parse(kv_drawable.Key),
                        Texture = int.Parse(kv_text.Key)
                    });
                }
                data.Add(int.Parse(kv_drawable.Key), drData);
            }
            return data;
        }

        public static Dictionary<int, int> LoadClothingTorsoDatafile(string json)
        {
            var data = new Dictionary<int, int>();
            string dataFile = LoadResourceFile("project_emergency_framework", json);
            var dataSerial = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, _CLOTHING_BEST_TORSO_ENTRY>>>(dataFile);
            foreach (var drawable in dataSerial)
            {
                data.Add(int.Parse(drawable.Key), int.Parse(drawable.Value["0"].BestTorsoDrawable));
            }
            return data;
        }

        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void ImportAllClothingData()
        {
            Accessories = LoadClothingDatafile("male_accessories.json");
            Hair_Male = LoadClothingDatafile("male_hair.json");
            Pants = LoadClothingDatafile("male_legs.json");
            Shoes = LoadClothingDatafile("male_shoes.json");
            Tops = LoadClothingDatafile("male_tops.json");
            Torsos = LoadClothingDatafile("male_torsos.json");
            Undershirts = LoadClothingDatafile("male_undershirts.json");
            Masks = LoadClothingDatafile("masks.json");
            Bracelets = LoadClothingDatafile("props_male_bracelets.json");
            Ears = LoadClothingDatafile("props_male_ears.json");
            Glasses = LoadClothingDatafile("props_male_glasses.json");
            Hats = LoadClothingDatafile("props_male_hats.json");
            Watches = LoadClothingDatafile("props_male_watches.json");

            BestTorsoForTop = LoadClothingTorsoDatafile("besttorso_male.json");

            string tattooDataFile = LoadResourceFile("project_emergency_framework", "tattoos.json");
            Tattoos = JsonConvert.DeserializeObject<List<TattooEntry>>(tattooDataFile);
        }
    }
    public class _CLOTHING_NAME_ENTRY
    {
        public string Localized { get; set; }
    }
    public class CrossRefEntry
    {
        public int Drawable { get; set; }
        public int Texture { get; set; }
    }
    public class _CLOTHING_BEST_TORSO_ENTRY
    {
        public string BestTorsoDrawable { get; set; }
    }
    public class TattooEntry
    {
        public int gender { get; set; }
        public string name { get; set; }
        public string collectionName { get; set; }
        public int zoneId { get; set; }
        public string zoneName { get; set; }
        public string type { get; set; }
    }
}
