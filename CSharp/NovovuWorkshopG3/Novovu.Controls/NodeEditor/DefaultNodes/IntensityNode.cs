using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor.DefaultNodes
{
    public class IntensityNode : NodeBox
    {
        private float _sliderValue = 0f;

        public float SliderValue
        {
            get
            {
                return _sliderValue;
            }
            set
            {
                _sliderValue = value;
                Outputs[0].GeneratedValue = value;
                //prevent stack overflows
                if (IntensitySlider.Value != value)
                {
                    IntensitySlider.SliderPos = value;
                }
            }
        }

        private IntensitySlider IntensitySlider;
        public IntensityNode() : base()
        {
            Title = "Intensity";
            Type = "SOURCE";
            Outputs = new List<NodePoint>()
            {
                new NodePoint("Intensity", "Intensity", 0, this, NodePoint.PointFlowTypes.Output)
            };
            NodeYOffset = 30;
            this.ReadyFeaturePaint += Paint;
            this.DragEnd = IntensityNode_DragEnd;
            this.Dragged = IntensityNode_Dragged;
            this.RequestDrag = IntensityNode_RequestDrag;
            Size = new Avalonia.Size(150, 100);
            IntensitySlider = new IntensitySlider(this);
        }

        private bool IntensityNode_RequestDrag(Avalonia.Point MousePosition, VectorCamera cam)
        {
            return IntensitySlider.DragBegin(MousePosition, cam.Position + Position + new Avalonia.Point(25, 40));
        }

        private bool IntensityNode_Dragged(Avalonia.Point MousePosition, VectorCamera cam)
        {
            IntensitySlider.MouseMoved(MousePosition, cam.Position + Position + new Avalonia.Point(25, 40));
            return true;
        }

        private bool IntensityNode_DragEnd(Avalonia.Point MousePosition, VectorCamera cam)
        {
            IntensitySlider.DragEnd();
            return true;
        }

        public void Paint(DrawingContext context, VectorCamera cam)
        {
            IntensitySlider.Draw(context, cam.Position + Position + new Avalonia.Point(25, 40));
        }
    }
}
