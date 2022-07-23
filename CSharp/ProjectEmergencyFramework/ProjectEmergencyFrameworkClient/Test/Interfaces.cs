using ProjectEmergencyFrameworkClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Test
{
    public class InterfaceUnitTest
    {
        public static bool TestRegister()
        {
            Interfaces.InterfaceController.REGISTER_INTERFACES();
            return true;
        }
        public static bool TestRegistered()
        {
            return Interfaces.InterfaceController.Interfaces.Count > 0;
        }
    }

    [UserInterface("test", false)]
    public class TestInterface : UserInterface
    {

    }
}
