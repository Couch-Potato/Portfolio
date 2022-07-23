using CitizenFX.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CitizenFX.Core.Native.API;
namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class PostalCodes
    {
        private static List<_POSTAL_CODE> _postal_codes = new List<_POSTAL_CODE>();
        public static Dictionary<string, Vector2> Postal = new Dictionary<string, Vector2>();
        [ExecuteAt(ExecutionStage.OnResourceStart)]
        public static void InitPostalZones()
        {
            string file = LoadResourceFile("project_emergency_framework", "postals.json");
            _postal_codes = JsonConvert.DeserializeObject<List<_POSTAL_CODE>>(file);
            foreach (var code in _postal_codes)
            {
                Postal.Add(code.code, new Vector2(code.x, code.y));
            }
        }

        public static string GetNearestPostalToPlayer()
        {
            var playerPos = Game.PlayerPed.Position;
            float minDistance = float.MaxValue;
            string nearest = "";
            foreach (var kvp in Postal)
            {
                float dist = Vector2.DistanceSquared(kvp.Value, new Vector2(playerPos.X, playerPos.Y));
                if (dist < minDistance)
                {
                    nearest = kvp.Key;
                    minDistance = dist;
                }
            }
            return nearest;
        }
        public static string GetNearestPostalToCoords(Vector3 coords)
        {
            float minDistance = float.MaxValue;
            string nearest = "";
            foreach (var kvp in Postal)
            {
                float dist = Vector2.Distance(kvp.Value, new Vector2(coords.X, coords.Y));
                if (dist < minDistance)
                {
                    nearest = kvp.Key;
                }
            }
            return nearest;
        }
        public static string GetNearestPostalToCoords(Vector2 coords)
        {
            float minDistance = float.MaxValue;
            string nearest = "";
            foreach (var kvp in Postal)
            {
                float dist = Vector2.Distance(kvp.Value, coords);
                if (dist < minDistance)
                {
                    nearest = kvp.Key;
                    minDistance = dist;
                }
            }
            return nearest;
        }
        public static Vector2 GetCoordsOfPostal(string postal)
        {
            return Postal[postal];
        }
    }
    public class _POSTAL_CODE {
        public float x { get; set; }
        public float y { get; set; }
        public string code { get; set; }
    }
}
