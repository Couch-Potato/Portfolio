using Microsoft.Xna.Framework;
using Novovu.Workshop.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Novovu.Workshop.Models
{
    public class GameObject:ITreeItem
    {
        public string Name { get; set; }

        [Property("Test", "", Property.PropertyType.String)]
        public string Test
        {
            get { return "123"; }
            set
            {
                Debug.WriteLine(value);
            }
        }
        [Property("Vector", "", Property.PropertyType.Vector3)]
        public Vector3 Vector
        {
            get => Vector3.Zero;
            set
            {
                Debug.WriteLine(value);
            }
        }
        public string TreeItemName => Name;

        public ObservableCollection<ITreeItem> Children { get; set; }

        public void TreeSelected()
        {
            Debug.WriteLine("Selected " + TreeItemName);
        }
    }
}
