using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Shared.Types
{
    public class Vector3
    {
        private string _x = "0";
        private string _y = "0";
        private string _z = "0";
        public string X { get => _x; set { _x = value; OnChanged?.Invoke(); } }
        public string Y { get => _y; set { _y = value; OnChanged?.Invoke(); } }
        public string Z { get => _z; set { _z = value; OnChanged?.Invoke(); } }
        public delegate void OnChangedHandler();
        public event OnChangedHandler OnChanged;

        
    }
}
