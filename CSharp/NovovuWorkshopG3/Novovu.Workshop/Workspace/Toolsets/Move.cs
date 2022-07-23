using Novovu.Workshop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace.Toolsets
{
    public class Move : ToolItem
    {
        public Move() 
            : base(
                  "MOVE_OFF.png",
                  "MOVE_ON.png"
                  )
        {
        }
    }
}
