using ProjectEmergencyFrameworkClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class Zones
    {
        public static Dictionary<string, string> ZoneDict = new Dictionary<string, string>();

        public static string GetZoneFullName(string zone)
        {
            return ZoneDict[zone.ToUpper()];
        }



        [ExecuteAt(ExecutionStage.Initialized)]
        public static void CreateZones()
        {
            ZoneDict.Add("AIRP", "Los Santos International Airport");
            ZoneDict.Add("ALAMO", "Alamo Sea");
            ZoneDict.Add("ALTA", "Alta");
            ZoneDict.Add("ARMYB", "Fort Zancudo");
            ZoneDict.Add("BANHAMC", "Banham Canyon Dr");
            ZoneDict.Add("BANNING", "Banning");
            ZoneDict.Add("BEACH", "Vespucci Beach");
            ZoneDict.Add("BHAMCA", "Banham Canyon");
            ZoneDict.Add("BRADP", "Braddock Pass");
            ZoneDict.Add("BRADT", "Braddock Tunnel");
            ZoneDict.Add("BURTON", "Burton");
            ZoneDict.Add("CALAFB", "Calafia Bridge");
            ZoneDict.Add("CANNY", "Raton Canyon");
            ZoneDict.Add("CCREAK", "Cassidy Creek");
            ZoneDict.Add("CHAMH", "Chamberlain Hills");
            ZoneDict.Add("CHIL", "Vinewood Hills");
            ZoneDict.Add("CHU", "Chumash");
            ZoneDict.Add("CMSW", "Chiliad Mountain State Wilderness");
            ZoneDict.Add("CYPRE", "Cypress Flats");
            ZoneDict.Add("DAVIS", "Davis");
            ZoneDict.Add("DELBE", "Del Perro Beach");
            ZoneDict.Add("DELPE", "Del Perro");
            ZoneDict.Add("DELSOL", "La Puerta");
            ZoneDict.Add("DESRT", "Grand Senora Desert");
            ZoneDict.Add("DOWNT", "Downtown");
            ZoneDict.Add("DTVINE", "Downtown Vinewood");
            ZoneDict.Add("EAST_V", "East Vinewood");
            ZoneDict.Add("EBURO", "El Burro Heights");
            ZoneDict.Add("ELGORL", "El Gordo Lighthouse");
            ZoneDict.Add("ELYSIAN", "Elysian Island");
            ZoneDict.Add("GALFISH", "Galilee");
            ZoneDict.Add("GOLF", "GWC and Golfing Society");
            ZoneDict.Add("GRAPES", "Grapeseed");
            ZoneDict.Add("GREATC", "Great Chaparral");
            ZoneDict.Add("HARMO", "Harmony");
            ZoneDict.Add("HAWICK", "Hawick");
            ZoneDict.Add("HORS", "Vinewood Racetrack");
            ZoneDict.Add("HUMLAB", "Humane Labs and Research");
            ZoneDict.Add("JAIL", "Bolingbroke Penitentiary");
            ZoneDict.Add("KOREAT", "Little Seoul");
            ZoneDict.Add("LACT", "Land Act Reservoir");
            ZoneDict.Add("LAGO", "Lago Zancudo");
            ZoneDict.Add("LDAM", "Land Act Dam");
            ZoneDict.Add("LEGSQU", "Legion Square");
            ZoneDict.Add("LMESA", "La Mesa");
            ZoneDict.Add("LOSPUER", "La Puerta");
            ZoneDict.Add("MIRR", "Mirror Park");
            ZoneDict.Add("MORN", "Morningwood");
            ZoneDict.Add("MOVIE", "Richards Majestic");
            ZoneDict.Add("MTCHIL", "Mount Chiliad");
            ZoneDict.Add("MTGORDO", "Mount Gordo");
            ZoneDict.Add("MTJOSE", "Mount Josiah");
            ZoneDict.Add("MURRI", "Murrieta Heights");
            ZoneDict.Add("NCHU", "North Chumash");
            ZoneDict.Add("NOOSE", "N.O.O.S.E");
            ZoneDict.Add("OCEANA", "Pacific Ocean");
            ZoneDict.Add("PALCOV", "Paleto Cove");
            ZoneDict.Add("PALETO", "Paleto Bay");
            ZoneDict.Add("PALFOR", "Paleto Forest");
            ZoneDict.Add("PALHIGH", "Palomino Highlands");
            ZoneDict.Add("PALMPOW", "Palmer-Taylor Power Station");
            ZoneDict.Add("PBLUFF", "Pacific Bluffs");
            ZoneDict.Add("PBOX", "Pillbox Hill");
            ZoneDict.Add("PROCOB", "Procopio Beach");
            ZoneDict.Add("RANCHO", "Rancho");
            ZoneDict.Add("RGLEN", "Richman Glen");
            ZoneDict.Add("RICHM", "Richman");
            ZoneDict.Add("ROCKF", "Rockford Hills");
            ZoneDict.Add("RTRAK", "Redwood Lights Track");
            ZoneDict.Add("SANAND", "San Andreas");
            ZoneDict.Add("SANCHIA", "San Chianski Mountain Range");
            ZoneDict.Add("SANDY", "Sandy Shores");
            ZoneDict.Add("SKID", "Mission Row");
            ZoneDict.Add("SLAB", "Stab City");
            ZoneDict.Add("STAD", "Maze Bank Arena");
            ZoneDict.Add("STRAW", "Strawberry");
            ZoneDict.Add("TATAMO", "Tataviam Mountains");
            ZoneDict.Add("TERMINA", "Terminal");
            ZoneDict.Add("TEXTI", "Textile City");
            ZoneDict.Add("TONGVAH", "Tongva Hills");
            ZoneDict.Add("TONGVAV", "Tongva Valley");
            ZoneDict.Add("VCANA", "Vespucci Canals");
            ZoneDict.Add("VESP", "Vespucci");
            ZoneDict.Add("VINE", "Vinewood");
            ZoneDict.Add("WINDF", "Ron Alternates Wind Farm");
            ZoneDict.Add("WVINE", "West Vinewood");
            ZoneDict.Add("ZANCUDO", "Zancudo River");
            ZoneDict.Add("ZP_ORT", "Port of South Los Santos");
            ZoneDict.Add("ZQ_UAR", "Davis Quartz");
            ZoneDict.Add("PROL", "Prologue / North Yankton");
            ZoneDict.Add("ISHeist", "Cayo Perico Island");
        }
    }
}
