using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WGameObject : Novovu.Xenon.Engine.GameObject, IContextable, Novovu.Workshop.Models.IIconable
    {

        public ContextOption[] ContextOptions { get; private set; }


        public new ObservableCollection<ITreeItem> Children
        {
            get
            {
                List<ITreeItem> child = new List<ITreeItem>();
                foreach (ITreeItem i in this.Models)
                {
                    child.Add(i);
                }
                foreach (ITreeItem i in this.Components)
                {
                    child.Add(i);
                }
               
                return new ObservableCollection<ITreeItem>(child);
            }
        }
        public WGameObject()
        {
            Mutex = Utility.RandomString(16);

            var Export = new ContextOption("Export Object");
            Export.ContextClick += async () =>
            {
                var savePlace = await ProjectStatic.SaveFile(ProjectStatic.NovovuObjectFilter);
                if (!string.IsNullOrWhiteSpace(savePlace))
                {
                    ProjectStatic.ExportObject(this, savePlace);
                }
            };
            var delete = new ContextOption("Delete");
            delete.ContextClick += () =>
            {
                LevelParent.Objects.Remove(this);
                ProjectStatic.InvokeRemoval(this);
            };

            var duplicate = new ContextOption("Duplicate");
            duplicate.ContextClick += () =>
            {
                var obj = Clone();
                LevelParent.Objects.Add(obj);
                ProjectStatic.InvokeDuplicate(obj);
            };

            var copy = new ContextOption("Copy");
            copy.ContextClick += () =>
            {
                ProjectStatic.InvokeObjectClipboardCopy(this);
            };
            var paste = new ContextOption("Paste");
            paste.ContextClick += () =>
            {
                ProjectStatic.InvokeObjectClipboardPaste(this);
            };

            var cut = new ContextOption("Cut");
            cut.ContextClick += () =>
            {
                ProjectStatic.InvokeObjectClipboardCopy(this);
                LevelParent.Objects.Remove(this);
                ProjectStatic.InvokeRemoval(this);
            };
            ContextOptions = new ContextOption[] { Export, delete, duplicate, copy, paste, cut };
        }
        public WGameObject Clone()
        {
            var obj = (WGameObject)this.MemberwiseClone();
            obj.Mutex = Utility.RandomString(16);
            List<BasicModel> bmdl = new List<BasicModel>();
            foreach (WBasicModel wbm in obj.Models)
            {
                var mdl = wbm.Clone();
                mdl.ParentObject = obj;
                bmdl.Add(mdl);
            }
            obj.Models = bmdl;
            ProjectAssets.ObjectModelSaved += (string name, WGameObject objx) =>
            {
                List<BasicModel> bmdlx = new List<BasicModel>();
                foreach (WBasicModel bm in objx.Models)
                {
                    var mdl = bm.Clone();
                    mdl.ParentObject = obj;
                    bmdlx.Add(mdl);
                }
                obj.Models = bmdlx;

            };
            return obj;
        }

        public string Mutex;

        public string IconSource => "avares://Novovu.Workshop/Assets/GAMEOBJECT.png";
    }
}
