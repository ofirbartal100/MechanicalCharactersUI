using System;
using MechanicalCharacters.Utils.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace MechanicalCharacters.Utils
{
    public interface IConnection : IDrawable
    {
        Point ConnectionPoint { get; set; }
        IComponent ConnectedComponent { get; set; }
    }

    public class ConnectionInfo
    {
        public bool IsSelected { get; set; }
    }

    public class PinConnection : IConnection, IDrawable
    {
        public ConnectionInfo Info { get; set; }

        public PinConnection(Point connectionPoint, IComponent connectedComponent, ConnectionInfo info = null)
        {
            ConnectionPoint = connectionPoint;
            ConnectedComponent = connectedComponent;

            Info = info ?? new ConnectionInfo();
        }

        public static void Connect(IComponent first, IComponent second, Point firstPoint, Point secondPoint)
        {
            if (first.Connections == null) first.Connections = new List<IConnection>();
            if (second.Connections == null) second.Connections = new List<IConnection>();

            first.Connections.Add(new PinConnection(firstPoint, second));
            second.Connections.Add(new PinConnection(secondPoint, first));
        }

        public Point ConnectionPoint { get; set; }
        public IComponent ConnectedComponent { get; set; }

        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();

            var pin = new Ellipse() { Width = 10, Height = 10, Margin = new Thickness(ConnectionPoint.X - 5, ConnectionPoint.Y - 5, 0, 0) };
            pin.Tag = Info;
            pin.Fill = Info.IsSelected ? Brushes.Orange : Brushes.DarkSlateBlue;
            pin.MouseEnter += (sender, args) =>
            {
                if (((ConnectionInfo)pin.Tag).IsSelected == false)
                    pin.Fill = Brushes.LightCyan;
            };
            pin.MouseLeave += (sender, args) =>
            {
                if (((ConnectionInfo)pin.Tag).IsSelected == false)
                    pin.Fill = Brushes.DarkSlateBlue;
            };
            pin.MouseUp += (sender, args) =>
            {
                if (((ConnectionInfo)pin.Tag).IsSelected == false)
                {
                    pin.Fill = Brushes.Orange;
                    ((ConnectionInfo)pin.Tag).IsSelected = true;
                }
                else
                {
                    pin.Fill = Brushes.DarkSlateBlue;
                    ((ConnectionInfo)pin.Tag).IsSelected = false;
                }
            };

            DrawingElements.Add(pin);

            return DrawingElements;
        }
    }
}