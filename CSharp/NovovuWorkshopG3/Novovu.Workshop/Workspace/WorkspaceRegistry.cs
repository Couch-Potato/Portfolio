using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public class WorkspaceRegistry
    {
        public static List<Workspace> Workspaces = new List<Workspace>();
        public static void ConstructWorkspaces()
        {
            Workspaces.Add(new ObjectCreator());
            Workspaces.Add(new ScriptEditor());
            Workspaces.Add(new LevelDesigner());
            
            
            
        }
        public static T GetWorkspace<T>()
        {
            foreach(var w in Workspaces)
            {
                if (w is T)
                {
                    return (T)Convert.ChangeType(w, typeof(T));
                }
            }
            return default;
        }
        public static void SetActive(Workspace w)
        {
            ActivationChanged?.Invoke(w);
        }
        public delegate void Revalidate(Workspace w);
        public static event Revalidate ActivationChanged;
    }
}
