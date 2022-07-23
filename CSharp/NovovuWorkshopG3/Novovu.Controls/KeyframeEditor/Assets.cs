using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.KeyframeEditor
{
    public class Assets
    {
        public static Bitmap SCRUBBER_BOX_EDGE;
        public static Bitmap SCRUBBER_BOX_EDGE_RIGHT;
        public static Bitmap SCRUBBER_BOX_FILL;
        public static Bitmap WHITE_BOX_EDGE;
        public static Bitmap WHITE_BOX_EDGE_RIGHT;
        public static Bitmap WHITE_BOX_FILL;
        public static Bitmap KEYFRAME_INNER_EDGE;
        public static Bitmap KEYFRAME_INNER_EDGE_RIGHT;
        public static Bitmap KEYFRAME_INNER_FILL;
        public static Bitmap KEYFROME_BAR_FILL;
        public static void Load()
        {
            SCRUBBER_BOX_EDGE = new Bitmap(File.OpenRead("Assets/EDGE1.png"));
            SCRUBBER_BOX_EDGE_RIGHT = new Bitmap(File.OpenRead("Assets/EDGE1-RIGHT.png"));
            SCRUBBER_BOX_FILL = new Bitmap(File.OpenRead("Assets/FILL1.png"));

            WHITE_BOX_EDGE = new Bitmap(File.OpenRead("Assets/EDGE2.png"));
            WHITE_BOX_EDGE_RIGHT = new Bitmap(File.OpenRead("Assets/EDGE2-RIGHT.png"));
            WHITE_BOX_FILL = new Bitmap(File.OpenRead("Assets/FILL2.png"));

            KEYFRAME_INNER_EDGE = new Bitmap(File.OpenRead("Assets/EDGE3.png"));
            KEYFRAME_INNER_EDGE_RIGHT = new Bitmap(File.OpenRead("Assets/EDGE3-RIGHT.png"));
            KEYFRAME_INNER_FILL = new Bitmap(File.OpenRead("Assets/FILL3.png"));

        }
    }
}
