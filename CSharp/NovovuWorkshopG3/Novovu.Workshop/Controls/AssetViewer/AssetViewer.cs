using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class AssetViewer : Control 
    {
        public List<IControlAssetTab> Tabs = new List<IControlAssetTab>();
        public AssetViewer()
        {
            Tabs = new List<IControlAssetTab>()
            {
                new AssetTab(),
                new AssetTab(),
            };
        }

        public override void Render(DrawingContext context)
        {

            foreach (var tab in ProjectModel.ProjectAssets.AssetTabs)
            {
                tab.Render(context, this);
            }
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            Dispatched = false;
            base.Render(context);
        }
        protected override Size MeasureCore(Size availableSize)
        {
            return availableSize;
        }
        public void SetActive(IControlAssetTab t)
        {
            foreach (var tab in ProjectModel.ProjectAssets.AssetTabs)
                if (!tab.Is(t))
                    tab.IsVisible = false;
        }
        bool Dispatched = false;
        public Pointer Pointer;
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (e.Pointer is Pointer)
            {
                Pointer = (Pointer)(e.Pointer);
            }
            foreach (var tab in ProjectModel.ProjectAssets.AssetTabs)
            {
                    tab.MouseMove(e.GetPosition(this));
            }
            if (!Dispatched)
            {
                
                Dispatched = true;
            }
            base.OnPointerMoved(e);
        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            foreach (var tab in ProjectModel.ProjectAssets.AssetTabs)
            {
                tab.MouseOneDown();
            }
            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            foreach (var tab in ProjectModel.ProjectAssets.AssetTabs)
            {
                tab.MouseOneUp();
            }
            base.OnPointerReleased(e);
        }

        public Point GetPositionForTabTitle(IControlAssetTab tab)
        {
            float TotalWidth = 14f;
            for (int i = 0; i < ProjectModel.ProjectAssets.AssetTabs.Count; i++)
            {
                if (ProjectModel.ProjectAssets.AssetTabs[i].Is(tab))
                {
                    return new Point(TotalWidth, 14);
                }
                TotalWidth += ProjectModel.ProjectAssets.AssetTabs[i].TitleWidth + 14f;
            }
            return new Point(14, 14);
        }
    }
}
