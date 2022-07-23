using Novovu.Workshop.Shared;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WCoreComponent :  ITreeItem, Models.IIconable
    {
        public IComponent Linked;
        public string Name { get; set; }
        public WCoreComponent(IComponent linked)
        {
            Linked = linked;
        }
        public string TreeItemName => Name;

        public ObservableCollection<ITreeItem> Children => new ObservableCollection<ITreeItem>();

        public string IconSource => "avares://Novovu.Workshop/Assets/COMPONENT.png";

        public dynamic Exports => Linked.Exports;

        public Dictionary<string, PropertyPair> Properties { get => Linked.Properties; set => Linked.Properties=value; }

        public bool Ran => Linked.Ran;

        public void Run(ComponentRunContext context)
        {
            Linked.Run(context);
        }

        public void RunChildComponent(IComponent comp, ComponentRunContext context)
        {
            Linked.RunChildComponent(comp, context);
        }

        public void Stop()
        {
            Linked.Stop();
        }

        public IComponent Clone()
        {
            return Linked.Clone();
        }
    }
}
