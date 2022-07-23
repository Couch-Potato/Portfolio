using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Utility
{
    public static class BaseIcons
    {
        public const string MissingIcon = "https://static.wikia.nocookie.net/minecraft_gamepedia/images/e/ef/Missing_Texture_JE3.png/revision/latest/scale-to-width-down/160?cb=20211013103047";
        public static string GetInventoryIcon(string name) {
            return $"/assets/inventory/{name}.svg";
        }
        public static string GetEffectIcon(string name)
        {
            return $"/assets/effects/{name}.svg";
        }
    }
}
