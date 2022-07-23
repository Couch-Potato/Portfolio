using Dock.Model.Controls;
using Novovu.Workshop.DockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class KeyFrameViewModel : Tool, IDockNameable
    {
        public string Title = "Keyframe Editor";

        public string Name => "Animator";
    }
}
