using Novovu.Workshop.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class ComponentRepoRoot : ITreeItem, Models.IIconable, IContextable
    {
        public string TreeItemName { get; }
        public ComponentRepoRoot(string repoName, bool ReadOnly = true)
        {
            TreeItemName = repoName;
            if (ReadOnly)
            {
                ContextOptions = new ContextOption[] { };
            }else
            {

                ContextOptions = new ContextOption[] { 
                    
                };
            }

        }
        public string IconSource => "avares://Novovu.Workshop/Assets/COMPONENT_REPO.png";
        public ObservableCollection<ITreeItem> Children => new ObservableCollection<ITreeItem>();

        public ContextOption[] ContextOptions {get;}
    }
}
