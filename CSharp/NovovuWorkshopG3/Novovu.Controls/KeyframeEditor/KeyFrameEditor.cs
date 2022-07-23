using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Novovu.Controls.KeyframeEditor.Scrubber;

namespace Novovu.Controls.KeyframeEditor
{
    public class KeyFrameEditor : Control
    {
        public string CurrentAnimationName = "Unnamed Animation";
        public Scrubber Scrubber = new Scrubber();
        public List<Layer> Layers = new List<Layer>();
        public KeyFrameEditor()
        {
            Assets.Load();
            Scrubber.ScrubberMoved += Scrubber_ScrubberMoved;
        }

        private void Scrubber_ScrubberMoved(float position)
        {
            ScrubberMoved?.Invoke(position);
        }

        public event ScrubberMoveDelegate ScrubberMoved; 

        public int GetIndexOfLayer(Layer l)
        {
            return Layers.IndexOf(l);
        }
        Brush BGBrush = new SolidColorBrush(new Color(255, 33, 35, 55));
        public override void Render(DrawingContext context)
        {
            context.DrawGeometry(BGBrush, null, new RectangleGeometry(new Rect(0, 0, 1920, 1080)));

            // Render Timeline
            FormattedText test = new FormattedText();
            test.Text = "TIMELINE";
            test.FontSize = 12;
            test.Typeface = new Typeface(FontFamily.Default,  FontStyle.Normal, FontWeight.Bold);
            context.DrawText(Brushes.White, new Avalonia.Point(5, 5), test);

            // Render Animation Name
            FormattedText name = new FormattedText();
            name.Text = CurrentAnimationName;
            name.FontSize = 12;
            name.Typeface = new Typeface(FontFamily.Default,  FontStyle.Normal, FontWeight.Bold);
            context.DrawText(Brushes.DarkGray, new Avalonia.Point(80, 5), name);

            // Render Scrubber Box
            context.DrawImage(Assets.SCRUBBER_BOX_EDGE,  new Rect(new Size(7, 64)), new Rect(
                new Point(127, 31),
                new Size(7, 7)
                ));
            context.DrawImage(Assets.SCRUBBER_BOX_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 7, 31),
                new Size(690 - 14, 7)
                ));
            context.DrawImage(Assets.SCRUBBER_BOX_EDGE_RIGHT,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 690 - 7, 31),
                new Size(7, 7)
                ));

            foreach (Layer l in Layers)
            {
                l.Render(context, this);
            }

            Scrubber.Render(context, this);

            Avalonia.Threading.Dispatcher.UIThread.Post(InvalidateVisual, Avalonia.Threading.DispatcherPriority.Render);

            base.Render(context);

        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                Scrubber.MouseDown(e.GetCurrentPoint(this).Position, this);
            }
            foreach (var layer in Layers)
            {
                layer.MouseDown(e.GetCurrentPoint(this).Position, this);
            }
            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {

            Scrubber.MouseUp(e.GetCurrentPoint(this).Position, this);
            foreach (var layer in Layers)
            {
                layer.MouseUp(e.GetCurrentPoint(this).Position, this);
            }
            base.OnPointerReleased(e);
        }
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            Scrubber.DragMoved(e.GetPosition(this), this);
            foreach (var layer in Layers)
            {
                layer.DragMoved(e.GetCurrentPoint(this).Position, this);
            }
            base.OnPointerMoved(e);
        }
    }
}
