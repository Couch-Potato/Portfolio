using Novovu.Controls.KeyframeEditor;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop
{
    public class Engine
    {
        public static EngineControlViewModel EngineControl = new EngineControlViewModel();
        public static EngineController ControlPlane;
        public static void GenerateControlPlane()
        {
            ControlPlane = new EngineController();
        }
        public static KeyFrameEditor Editor = new KeyFrameEditor();
        public static KeyFrameEditor GenerateKeyFrameEditor()
        {
            Editor = new KeyFrameEditor();
            return Editor;
        }
    }
}
