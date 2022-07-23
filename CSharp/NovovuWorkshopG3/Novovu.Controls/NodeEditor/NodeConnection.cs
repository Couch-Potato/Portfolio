using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Novovu.Controls.NodeEditor
{
    public class NodeConnection
    {
        [XmlIgnoreAttribute]
        public NodePoint Input;
        [XmlIgnore]
        public NodePoint Output;

        public Point ResultPoint;

        IPen pen = new Pen(Brushes.White, 3);

        public void SetQuasiPoint(Point p)
        {
            ResultPoint = p;
        }

        public void Draw(DrawingContext context, VectorCamera camera)
        {
            if (Input != null && Output != null)
            {
                const int interpolation = 48;

                var input = Input.ConnectionPoint + camera.Position;
                var output = Output.ConnectionPoint + camera.Position;

                Point[] points = new Point[interpolation];

                for (int i = 0; i < interpolation; i++)
                {
                    double amount = i / (double)(interpolation - 1);
                    var lx = Lerp(output.X, input.X, amount);
                    var d = Math.Min(Math.Abs(input.X - output.X), 100);
                    var a = new Point(Scale(amount, 0, 1, output.X, output.X + d), output.Y);
                    var b = new Point(Scale(amount, 0, 1, input.X - d, input.X), input.Y);
                    var bas = Sat(Scale(amount, 0.1, 0.9, 0, 1));
                    var cos = Math.Cos(bas * Math.PI);
                    if (cos < 0)
                    {
                        cos = -Math.Pow(-cos, 0.2);
                    }
                    else
                    {
                        cos = Math.Pow(cos, 0.2);
                    }
                    amount = cos * -0.5f + 0.5f;

                    var f = Lerp(a, b, amount);
                    points[i] = f;

                }

                for (int i = 1; i < interpolation - 1; i += 2)
                {
                    context.DrawLine(pen, points[i], points[i + 1]);
                }
                for (int i = 0; i < interpolation; i += 2)
                {
                    context.DrawLine(pen, points[i], points[i + 1]);
                }
                // context.DrawLine(pen, Input.ConnectionPoint + camera.Position, Output.ConnectionPoint + camera.Position);

            }
            else if (Input != null && ResultPoint != null)
            {
                const int interpolation = 48;

                var input = Input.ConnectionPoint + camera.Position;
                var output = ResultPoint;

                Point[] points = new Point[interpolation];

                for (int i = 0; i < interpolation; i++)
                {
                    double amount = i / (double)(interpolation - 1);
                    var lx = Lerp(output.X, input.X, amount);
                    var d = Math.Min(Math.Abs(input.X - output.X), 100);
                    var a = new Point(Scale(amount, 0, 1, output.X, output.X + d), output.Y);
                    var b = new Point(Scale(amount, 0, 1, input.X - d, input.X), input.Y);
                    var bas = Sat(Scale(amount, 0.1, 0.9, 0, 1));
                    var cos = Math.Cos(bas * Math.PI);
                    if (cos < 0)
                    {
                        cos = -Math.Pow(-cos, 0.2);
                    }
                    else
                    {
                        cos = Math.Pow(cos, 0.2);
                    }
                    amount = cos * -0.5f + 0.5f;

                    var f = Lerp(a, b, amount);
                    points[i] = f;

                }

                for (int i = 1; i < interpolation - 1; i += 2)
                {
                    context.DrawLine(pen, points[i], points[i + 1]);
                }
                for (int i = 0; i < interpolation; i += 2)
                {
                    context.DrawLine(pen, points[i], points[i + 1]);
                }
            }

        }
        public static double Sat(double x)
        {
            if (x < 0) return 0;
            if (x > 1) return 1;
            return x;
        }
        public static double Lerp(double a, double b, double amount)
        {
            return a * (1f - amount) + b * amount;
        }
        public static Point Lerp(Point a, Point b, double amount)
        {

            var x = a.X * (1 - amount) + b.X * amount;
            var y = a.Y * (1 - amount) + b.Y * amount;
            return new Point(x, y);
        }
        public static double Scale(double x, double a, double b, double c, double d)
        {
            double s = (x - a) / (b - a);
            return s * (d - c) + c;
        }
        public void Terminate()
        {
            Input.Connection = null;
            if (Output != null)
            {
                Output.Connection = null;
            }


        }
    }
}
