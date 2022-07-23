using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace.Handlers
{
    class H_LOAD_TOOLS:IWorkspaceHandle
    {
        public int ToolId = 0;
        public H_LOAD_TOOLS(int id) { ToolId = id; }

    }
}
