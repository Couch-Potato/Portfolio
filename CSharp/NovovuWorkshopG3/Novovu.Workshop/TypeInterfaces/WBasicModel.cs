using Novovu.Workshop.Controls;
using Novovu.Workshop.Models;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    [System.Xml.Serialization.XmlInclude(typeof(WBasicModel))]
    public class WBasicModel : Novovu.Xenon.Engine.BasicModel, IContextable, IIconable
    {
        public ContextOption[] ContextOptions { get; }
        public WBasicModel()
        {
            Mutex = Utility.RandomString(16);

            var delete = new ContextOption("Delete");
            delete.ContextClick += () =>
            {
                ParentObject.Models.Remove(this);
                ProjectStatic.InvokeRemoval(this);
            };

            var duplicate = new ContextOption("Duplicate");
            duplicate.ContextClick += () =>
            {
                var obj = (WBasicModel)this.MemberwiseClone();
                obj.Mutex = Utility.RandomString(16);
                ParentObject.Models.Add(obj);
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
                ParentObject.Models.Remove(this);
                ProjectStatic.InvokeRemoval(this);
            };
            ContextOptions = new ContextOption[] { delete, duplicate, copy, paste, cut };

        }
        public WBasicModel Clone()
        {
            var obj = (WBasicModel)this.MemberwiseClone();
            obj.Mutex = Utility.RandomString(16);
            return obj;
        }

        public string Mutex;
        public string IconSource => "avares://Novovu.Workshop/Assets/3DOBJECT.png";

        private Views.MaterialCreator MaterialCreator;
        private MaterialNodeEditor editor;

        [Property("Material", "Basic", Property.PropertyType.Execute)]
        public Shared.ExecutableProperty GetMaterial => () =>
        {
            MaterialCreator = new Views.MaterialCreator(this);
            
            MaterialCreator.Show();
        };

    }
}
