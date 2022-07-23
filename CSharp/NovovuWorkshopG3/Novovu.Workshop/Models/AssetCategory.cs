using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Novovu.Workshop.Models
{
    [Obsolete]
    public class AssetCategory
    {
        public string CategoryName { get; set; }
        public ObservableCollection<AssetItem> Items { get; set; }

        public delegate void ItemInsertHandler(AssetItem item);
        public event ItemInsertHandler ItemInsertRequested;
        public void InvokeInsert(AssetItem item)
        {
            ItemInsertRequested?.Invoke(item);
        }

    }
}
