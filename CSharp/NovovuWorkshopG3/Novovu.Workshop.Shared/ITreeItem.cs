using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Shared
{
    public interface ITreeItem
    {
        string TreeItemName { get; }
        ObservableCollection<ITreeItem> Children { get; }
       // void TreeSelected();

    }
}
