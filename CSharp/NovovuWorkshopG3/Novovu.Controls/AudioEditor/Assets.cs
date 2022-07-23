using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.AudioEditor
{
    public class Assets
    {
        public static Bitmap BAR_BACKGROUND;
        public static Bitmap CIRCLE;
        public static Bitmap EQRANGE;

        public static void Load()
        {
            BAR_BACKGROUND = new Bitmap(File.OpenRead("Assets/BARBG.png"));
            CIRCLE = new Bitmap(File.OpenRead("Assets/CIRCLE.png"));
            EQRANGE = new Bitmap(File.OpenRead("Assets/EQRANGE.png"));
        }
    }
}
