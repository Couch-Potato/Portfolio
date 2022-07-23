
using Novovu.Workshop.Controls.AssetViewer;
using Novovu.Workshop.Models;
using Novovu.Workshop.TypeInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Novovu.Workshop.ProjectModel
{
    public class ProjectAssets
    {
        public delegate void ObjectModelSavedDelegate(string name, WGameObject obj);
        public static event ObjectModelSavedDelegate ObjectModelSaved;
        public static void SaveObjectModel(string name, WGameObject obj)
        {
            ObjectModelSaved?.Invoke(name, obj);
        }
        /*[Obsolete]
        public static List<AssetCategory> Assets = new List<AssetCategory>()
        {
            new AssetCategory()
                {
                    CategoryName="Objects",
                    Items = new System.Collections.ObjectModel.ObservableCollection<AssetItem>()

                },
             new AssetCategory()
                {
                    CategoryName="Models",
                    Items = new System.Collections.ObjectModel.ObservableCollection<AssetItem>()

                }
             ,
             new AssetCategory()
                {
                    CategoryName="Components",
                    Items = new System.Collections.ObjectModel.ObservableCollection<AssetItem>()

                }
        };*/
        public static WComponentRepositories ComponentRepositories = new WComponentRepositories();
        public static List<IControlAssetTab> AssetTabs = new List<IControlAssetTab>
        {
            new Controls.AssetViewer.AssetTab
            {
                TabName = "Objects"
            },
            new Controls.AssetViewer.AssetTab
            {
                TabName = "Models"
            },
            ComponentRepositories,
            new Controls.AssetViewer.AssetTab
            {
                TabName = "Materials"
            },
            new Controls.AssetViewer.AssetTab
            {
                TabName = "Audio"
            },
        }
        ;
       /* [Obsolete]
        public static int GetIndexOfCategory(string name)
        {
            for (int i=0;i<Assets.Count;i++)
                if (Assets[i].CategoryName == name)
                    return i;
            return -1;
        }*/
        public static int GetIndexOfTab(string name)
        {
            for (int i=0; i<AssetTabs.Count;i++)
            {
                if (AssetTabs[i].TabName == name)
                    return i;
            }
            return -1;
        }
       /* [Obsolete]
        public static int GetIndexOfAsset(int category, string name)
        {
            for (int i = 0; i < Assets[category].Items.Count; i++)
                if (Assets[category].Items[i].ItemName == name)
                    return i;
            return -1;
        }*/
        public static int GetIndexOfAssetId(int category, string name)
        {
            for (int i = 0; i < AssetTabs[category].Items.Count; i++)
                if (AssetTabs[category].Items[i].Name == name)
                    return i;
            return -1;
        }
        /*[Obsolete]
        public static void AddAssetToCategory(string asset, AssetItem item)
        {
            foreach (AssetCategory cat in Assets)
            {
                if (cat.CategoryName == asset)
                {
                    item.Parent = cat;
                    cat.Items.Add(item);
                }
                    
            }
        }*/
        public static void AddAssetToTab(string tab, Controls.AssetViewer.AssetItem item)
        {
            foreach (IControlAssetTab cat in AssetTabs)
            {
                if (cat.TabName == tab)
                {
                    cat.Items.Add(item);
                }

            }
        }
        /*[Obsolete]
        public static AssetCategory GetCategory(string name)
        {
            foreach (AssetCategory a in Assets)
                if (a.CategoryName == name)
                    return a;
            return null;
        }*/

        public static IControlAssetTab GetTab(string name)
        {
            foreach (IControlAssetTab a in AssetTabs)
                if (a.TabName == name)
                    return a;
            return null;
        }

       /* [Obsolete]
        public static AssetCategory[] GetProjectAssets()
        {
            return Assets.ToArray();
        }*/
    }
}
