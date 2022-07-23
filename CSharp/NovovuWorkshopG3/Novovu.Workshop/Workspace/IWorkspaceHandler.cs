using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public interface IWorkspaceHandler
    {
        void Handle(IWorkspaceHandle Handle);
    }
}
