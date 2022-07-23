using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.UI
{
    [UserInterface("pld")]
    public class PLD : UserInterface
    {
        public string direction { get; set; } = "N";
        public string postal { get; set; } = "0";
        public string location { get; set; } = "";
        public bool onTop { get; set; } = true;
    }
}
