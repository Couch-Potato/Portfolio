using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.Animation
{
    public class Animator
    {
        public static float Quart(float x)
        {
            return 1 - (float)Math.Pow(1 - x, 4);
        }
        public static Point AnimateQuart(float elapsed, float complete, Point a, Point b)
        {
            var inc = b - a;
            return a + (inc * Quart(elapsed / complete));
            //return Quart(elapsed / complete);
        }
    }
}
