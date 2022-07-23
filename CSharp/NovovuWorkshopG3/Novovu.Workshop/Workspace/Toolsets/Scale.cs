using Novovu.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace.Toolsets
{
    public class Scale: ToolItem
    {
        public Scale() : base(
                  "SCALE_OFF.png",
                  "SCALE_ON.png"
                  )
        { }
    }
}
