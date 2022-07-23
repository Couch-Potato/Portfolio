using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Utilities;
using Novovu.Controls.ContextMenu;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using ContextMenu = Novovu.Controls.ContextMenu.ContextMenu;

namespace Novovu.Controls.NodeEditor
{
    public abstract class NodeEditor : UserControl
    {
        Bitmap Background;
        VectorCamera Camera = new VectorCamera();
        public NodeEditor()
        {
            this.Initialized += NodeEditor_Initialized;
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            Background = new Bitmap(File.OpenRead("Assets/gpx2.jpg"));
            this.PointerPressed += NodeEditor_PointerPressed;
            this.PointerMoved += NodeEditor_PointerMoved;
            this.PointerReleased += NodeEditor_PointerReleased;
            this.KeyDown += NodeEditor_KeyDown;
            NodeConnection nc = new NodeConnection();

            ContextMenuItem AddIntensityNode = new ContextMenuItem();
            AddIntensityNode.Name = "Add Intensity";
            AddIntensityNode.OnSelect += AddIntensityNode_OnSelect;

            ContextMenuItem AddTextureNode = new ContextMenuItem();
            AddTextureNode.Name = "Add Texture";
            AddTextureNode.OnSelect += AddTextureNode_OnSelect;

            ContextMenuItem SaveMenu = new ContextMenuItem();
            SaveMenu.Name = "Save";
            SaveMenu.OnSelect += SaveMenu_OnSelect;

            NovovuContextMenu.Items.Add(AddIntensityNode);
            NovovuContextMenu.Items.Add(AddTextureNode);
            NovovuContextMenu.Items.Add(SaveMenu);
        }

        private void SaveMenu_OnSelect()
        {
            Save();
        }

        private void AddTextureNode_OnSelect()
        {
            var node = new DefaultNodes.TextureNode();
            CreateBox(node, MouseClickPosition);
        }

        private void AddIntensityNode_OnSelect()
        {
            var node = new DefaultNodes.IntensityNode();
            CreateBox(node, MouseClickPosition);
        }

        public void CreateBox(NodeBox box, Point Position = default)
        {
            box.Position = Position;
            Boxes.Add(box);
            box.OnRemove += Boxx_OnRemove;
        }

        private void NodeEditor_KeyDown(object sender, Avalonia.Input.KeyEventArgs e)
        {


        }
        Point MouseClickPosition;
        ContextMenu.ContextMenu NovovuContextMenu = new ContextMenu.ContextMenu();
        public List<ContextMenuItem> ContextMenuItems
        {
            get => NovovuContextMenu.Items;
            set
            {
                NovovuContextMenu.Items = value;
                NovovuContextMenu.Hide();
            }
        }
        private void Boxx_OnRemove(NodeBox box)
        {

            Boxes.Remove(box);
        }

        public List<NodeBox> Boxes = new List<NodeBox>();
        bool wasRightPressed = false;
        private void NodeEditor_PointerReleased(object sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            var ptx = e.GetCurrentPoint(this);
            if (!wasRightPressed)
            {
                NovovuContextMenu.Click(e.GetPosition(this));
            }
            if (wasRightPressed)
            {
                MouseClickPosition = e.GetPosition(this);
                NovovuContextMenu.invpos = e.GetPosition(this);
                NovovuContextMenu.Show();
                wasRightPressed = false;
            }
            AnyItemBeingDragged = false;
            var pt = e.GetCurrentPoint(this).Position;
            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                foreach (NodeBox Box in Boxes)
                {
                    Box.DragStop(pt, Camera);
                    if (Box.Drag)
                        Box.Drag = false;
                }


                MouseOneDown = false;
            }
            //Serializer.Export(this, "test.xml");
        }
        Point DownPosition;
        Point MousePosition;
        bool AnyItemBeingDragged = false;

        protected abstract void Save();
        private void NodeEditor_PointerMoved(object sender, Avalonia.Input.PointerEventArgs e)
        {
            var pt = e.GetCurrentPoint(this).Position;
            MousePosition = pt;
            NovovuContextMenu.MouseMove(e.GetPosition(this));
            if (MouseOneDown)
            {
                var difference = (pt - DownPosition);
                if (!AnyItemBeingDragged)
                {
                    Camera.Position += difference;
                    DownPosition = pt;
                }

            }
            foreach (NodeBox Box in Boxes)
            {
                Box.DraggedMouse(pt, Camera);
                Box.Hover(pt, Camera);
                if (Box.Drag)
                {

                    var difference = (pt - DownPosition);
                    DownPosition = pt;
                    Box.Position += difference;
                    return;
                }

            }

        }

        bool MouseOneDown = false;
        private void NodeEditor_PointerPressed(object sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var pos = e.GetCurrentPoint(this).Position;
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                foreach (NodeBox Box in Boxes)
                {
                    var x = Box.DragBegin(pos, Camera);
                    if (x)
                    {
                        AnyItemBeingDragged = true;
                    }
                    if (Box.IsMouseOverHeader(pos, Camera))
                    {
                        Box.Drag = true;
                        DownPosition = e.GetCurrentPoint(this).Position;
                        return;
                    }
                    else
                    {
                        Box.Click(pos, Camera);
                    }


                }

                MouseOneDown = true;
                DownPosition = e.GetCurrentPoint(this).Position;
            }else
            {
                wasRightPressed = true;
            }

        }

        protected override Size MeasureCore(Size availableSize)
        {
            Width = availableSize.Width;
            Height = availableSize.Height;
            return base.MeasureCore(availableSize);
        }

        private void NodeEditor_Initialized(object sender, EventArgs e)
        {
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
        }

        bool ControlDown = false;
        protected override void OnKeyDown(Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.LeftCtrl)
                ControlDown = true;
           // base.OnKeyDown(e);
        }
        protected override void OnKeyUp(Avalonia.Input.KeyEventArgs e)
        {
            if (e.Key == Avalonia.Input.Key.S && ControlDown)
            {
                Save();
            }
            if (e.Key == Avalonia.Input.Key.LeftCtrl)
                ControlDown = false;
           // base.OnKeyUp(e);
        }

        public override void Render(DrawingContext context)
        {
            

            int drawX = (int)MathUtilities.Clamp(Math.Ceiling((Width) / (450)), 1, int.MaxValue);
            int drawY = (int)MathUtilities.Clamp(Math.Ceiling((Height) / (274)), 1, int.MaxValue);

            for (int i = 0; i < drawX; i++)
            {
                for (int j = 0; j < drawY; j++)
                {
                    context.DrawImage(Background,  new Rect((i * 450), (j * 274), 450, 274));
                }

            }
            foreach (var Box in Boxes)
            {
                Box.Draw(context, Camera);
            }

            ConnectionFactory.Draw(context, Camera, MousePosition);
            NovovuContextMenu.Draw(context);
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);

            base.Render(context);

        }

    }
}
