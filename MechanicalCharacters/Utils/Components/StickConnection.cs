using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils.Components
{
    public class StickConnection : IDrawable
    {
        public const double height = 30;
        public StickConnection(double x, double y, double rotation = 0, double lenght = 100, string id = "")
        {
            Anchor = new Point(x, y);
            RotationDegree = rotation % 360;
            Length = lenght;
            ID = id;

            StickPointInfos = new List<StickPointInfo>(){new StickPointInfo(){ID = ID,StickPointType = 0}, new StickPointInfo() { ID = ID, StickPointType = 1 } };
        }

        public Point Anchor { get; set; }
        public StickConnection ChildConnection { get; set; }
        public string ID { get; set; }
        public double Length { get; set; }
        public double RotationDegree { get; set; }
        public List<StickPointInfo> StickPointInfos { get; set; }
        public void AddChild(double degree = 0, double length = 100)
        {
            var connectionPoint = new Point(Anchor.X + Length * 0.8 * Math.Cos(RotationDegree * Math.PI / 180),
                Anchor.Y + Length * 0.8 * Math.Sin(RotationDegree * Math.PI / 180));
            ChildConnection = new StickConnection(connectionPoint.X, connectionPoint.Y, degree + RotationDegree, length);
        }

        public void AddLastChild(double degree = 0, double length = 100)
        {
            if (ChildConnection != null)
                ChildConnection.AddLastChild(degree, length);
            else
                AddChild(degree, length);
        }

        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();

            var tran = new TranslateTransform(Anchor.X - Length / 10, Anchor.Y - height / 2);
            var transform = new RotateTransform(RotationDegree, Anchor.X, Anchor.Y);
            TransformGroup t = new TransformGroup
            {
                Children = new TransformCollection(new Transform[] { tran, transform })
            };
            var rect = new Rectangle() { Width = Length, Height = height, Fill = Brushes.LightCoral, RenderTransform = t };
            DrawingElements.Add(rect);
            var pin = new Ellipse() { Tag = StickPointInfos[0], Width = 10, Height = 10, Fill = StickPointInfos[0].IsSelected ? Brushes.Orange : Brushes.DarkSlateBlue, Margin = new Thickness(Anchor.X - 5, Anchor.Y - 5, 0, 0) };

            pin.MouseEnter += (sender, args) =>
            {
                if (((StickPointInfo)pin.Tag).IsSelected == false)
                    pin.Fill = Brushes.LightCyan;
            };
            pin.MouseLeave += (sender, args) =>
            {
                if (((StickPointInfo)pin.Tag).IsSelected == false)
                    pin.Fill = Brushes.DarkSlateBlue;
            };
            pin.MouseUp += (sender, args) =>
            {
                if (((StickPointInfo)pin.Tag).IsSelected == false)
                {
                    pin.Fill = Brushes.Orange;
                    ((StickPointInfo)pin.Tag).IsSelected = true;
                }
                else
                {
                    pin.Fill = Brushes.DarkSlateBlue;
                    ((StickPointInfo)pin.Tag).IsSelected = false;
                }
                //((StickPointInfo)pin.Tag).IsSelected = !((StickPointInfo)pin.Tag).IsSelected;
            };

            DrawingElements.Add(pin);

            if (ChildConnection != null)
                DrawingElements.AddRange(ChildConnection.GetDrawing());
            else
            {
                var endPoint = new Point(Anchor.X + Length * 0.8 * Math.Cos(RotationDegree * Math.PI / 180),
                    Anchor.Y + Length * 0.8 * Math.Sin(RotationDegree * Math.PI / 180));
                var pinEnd = new Ellipse() { Tag = StickPointInfos[1], Width = 10, Height = 10, Fill = StickPointInfos[1].IsSelected ? Brushes.Orange : Brushes.DarkSlateBlue, Margin = new Thickness(endPoint.X - 5, endPoint.Y - 5, 0, 0) };

                pinEnd.MouseEnter += (sender, args) =>
                {
                    if (((StickPointInfo)pinEnd.Tag).IsSelected == false)
                        pinEnd.Fill = Brushes.LightCyan;
                };
                pinEnd.MouseLeave += (sender, args) =>
                {
                    if (((StickPointInfo)pinEnd.Tag).IsSelected == false)
                        pinEnd.Fill = Brushes.DarkSlateBlue;
                };
                pinEnd.MouseUp += (sender, args) =>
                {
                    if (((StickPointInfo)pinEnd.Tag).IsSelected == false)
                    {
                        pinEnd.Fill = Brushes.Orange;
                        ((StickPointInfo)pinEnd.Tag).IsSelected = true;
                    }
                    else
                    {
                        pinEnd.Fill = Brushes.DarkSlateBlue;
                        ((StickPointInfo)pinEnd.Tag).IsSelected = false;
                    }
                    //((StickPointInfo)pinEnd.Tag).IsSelected = !((StickPointInfo)pinEnd.Tag).IsSelected;
                };

                DrawingElements.Add(pinEnd);
            }

            return DrawingElements;
        }

        public void Rotate(double degrees)
        {
            RotationDegree += degrees;
            if (ChildConnection != null)
                ForwardKinematics(this, ChildConnection, degrees);
        }

        public void SetRotation(double degree)
        {
            Rotate(degree - RotationDegree);
        }

        private void ForwardKinematics(StickConnection parent, StickConnection childConnection, double degrees)
        {
            if (childConnection != null)
            {
                var connectionPoint = new Point(parent.Anchor.X + parent.Length * 0.8 * Math.Cos(parent.RotationDegree * Math.PI / 180),
                    parent.Anchor.Y + parent.Length * 0.8 * Math.Sin(parent.RotationDegree * Math.PI / 180));

                childConnection.Anchor = connectionPoint;
                childConnection.RotationDegree += degrees;
                ForwardKinematics(childConnection, childConnection.ChildConnection, degrees);
            }
        }
    }

    public class StickPointInfo
    {
        //0 for anchor, 1 for endpoint
        public int StickPointType = 0;
        public string ID { get; set; }
        public bool IsSelected { get; set; } = false;
    }
}