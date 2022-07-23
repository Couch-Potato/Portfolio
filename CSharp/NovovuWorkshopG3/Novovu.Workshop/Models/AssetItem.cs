using Novovu.Xenon.Assets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Novovu.Workshop.Models
{
    [Obsolete]
    public class AssetItem
    {
        public string ItemName { get; set; }
        public string ItemIcon { get; set; }

        public Asset ItemLink { get; set; }

        public object Attachment { get; set; }

        public delegate void OnSelect();

        public event OnSelect Selected;

        public AssetCategory Parent;

        public void SelectItem()
        {
            Selected?.Invoke();
            Parent?.InvokeInsert(this);
        }

    }
}
