using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.AudioEditor
{
    public class EQControl : Control
    {
        public List<Slider> Sliders = new List<Slider>()
        {
            new Slider(100,100,"30"),
            new Slider(150,100, "60"),
            new Slider(200,100, "120"),
            new Slider(250,100, "250"),
            new Slider(300,100, "500"),
            new Slider(350,100, "1K"),
            new Slider(400,100, "2K"),
            new Slider(450,100, "4K"),
            new Slider(500,100, "8K"),
            new Slider(550,100, "16K"),
        };
        public EQControl()
        {
            Assets.Load();
        }
        public override void Render(DrawingContext context)
        {
            foreach (Slider s in Sliders)
            {
                s.Draw(context);
            }
            Dispatcher.UIThread.Post(InvalidateVisual, DispatcherPriority.Render);
            base.Render(context);
        }
        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                var pos = e.GetCurrentPoint(this).Position;
                foreach (Slider s in Sliders)
                {
                    s.MouseDown(pos);
                }
            }

            base.OnPointerPressed(e);
        }
        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {

            var pos = e.GetCurrentPoint(this).Position;
            foreach (Slider s in Sliders)
            {
                s.MouseUp(pos);
            }
            
            base.OnPointerReleased(e);
        }
        protected override void OnPointerMoved(PointerEventArgs e)
        {
            var pos = e.GetCurrentPoint(this).Position;
            foreach (Slider s in Sliders)
            {
                s.MouseMove(pos);
            }
            
            base.OnPointerMoved(e);
        }
    }
}
