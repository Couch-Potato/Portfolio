using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor
{
    public class ConnectionFactory
    {
        public static bool ConnectionPending = false;
        public static NodePoint Connectable;
        public static bool IsOutput;
        private static NodeConnection QuasiConnection;
        public static void Connect(NodePoint point)
        {
            if (ConnectionPending)
            {
                if (point.PointFlow == NodePoint.PointFlowTypes.Output && IsOutput || point.PointFlow == NodePoint.PointFlowTypes.Input && !IsOutput)
                    return;
                point.Connection?.Terminate();
                Connectable.Connection?.Terminate();
                ConnectionPending = false;
                NodeConnection connection = new NodeConnection();
                if (IsOutput)
                {
                    connection.Output = Connectable;
                    connection.Input = point;
                }
                else
                {
                    connection.Output = point;
                    connection.Input = Connectable;
                }
                point.Connection = connection;
                connection.Input.GeneratedValue = connection.Output.GeneratedValue;
                Connectable.Connection = connection;
                QuasiConnection = null;
            }
            else
            {
                ConnectionPending = true;
                Connectable = point;
                IsOutput = point.PointFlow == NodePoint.PointFlowTypes.Output;
                QuasiConnection = new NodeConnection();
                QuasiConnection.ResultPoint = point.Parent.GetPositionFromIndex(point);
                QuasiConnection.Input = point;
                point.Connection = QuasiConnection;
            }
        }
        public static void Draw(DrawingContext context, VectorCamera cam, Point mousepos)
        {
            QuasiConnection?.SetQuasiPoint(mousepos);
            QuasiConnection?.Draw(context, cam);
        }

    }
}
