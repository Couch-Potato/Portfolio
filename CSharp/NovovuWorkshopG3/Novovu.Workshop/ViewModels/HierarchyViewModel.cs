using Avalonia.Controls;
using Dock.Model.Controls;
using Novovu.Workshop.Models;
using Novovu.Workshop.Shared;
using Novovu.Workshop.Workspace;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

namespace Novovu.Workshop.ViewModels
{
    public class HierarchyViewModel : Tool, DockSystem.IDockNameable
    {
        private string _t = "Hierarchy";
        public string Title = "Hierarchy";
        public string HierarchyTitle
        {
            get => _t;
            set => this.RaiseAndSetIfChanged(ref _t, value);
        }

        public ObservableCollection<ITreeItem> GameObjects { get; }

        public string Name => "Hierarchy";

        public WorkspaceInterface WSIF;
        public HierarchyViewModel()
        {
            
            GameObjects = new ObservableCollection<ITreeItem>();
            
        }

        private List<GameObject> Test()
        {
            return new List<GameObject>()
            {
                new GameObject()
                {
                    Name="Lone"
                },
                new GameObject()
                {
                    Name="Parent",
                    Children=new ObservableCollection<ITreeItem>()
                    {
                        new GameObject()
                        {
                            Name="Lone"
                        }
                    }
                }
            };
        }

        public TreeView LinkedTree;
        bool IgnoreUpdate = false;
        public void SelectItems(object items)
        {
            IgnoreUpdate = true;
            LinkedTree.SelectedItem = items;
        }

        public List<object> SelectedItems = new List<object>();
        public delegate void SelectionEventHandler(object[] selected);
        public event SelectionEventHandler SelectionUpdated;
        public void HandleSelect(object sender, SelectionChangedEventArgs e)
        {
            if (IgnoreUpdate)
            {
                IgnoreUpdate = false;
                return;
            }
            foreach (object a in e.AddedItems)
            {
                SelectedItems.Clear();
                SelectedItems.Add(a);
            }
            foreach (object a in e.RemovedItems)
            {
                SelectedItems.Remove(a);
            }
            if (SelectedItems.Count == 0)
                return;
            if (SelectedItems[0] is ITreeItem)
            {
                var ix = (ITreeItem)SelectedItems[0];
                //ix.TreeSelected();
                SelectionUpdated?.Invoke(SelectedItems.ToArray());
                e.Handled = true;
            }
        }
    }
}
