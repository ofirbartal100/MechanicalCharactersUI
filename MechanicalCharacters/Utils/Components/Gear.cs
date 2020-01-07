using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils.Components
{
    public class Gear : IDrawable, IComponent
    {
        public Point Center { get; set; }
        public double Radius { get; set; }
        private int _phase;

        public int Phase
        {
            get { return _phase; }
            set { _phase = (value + 1000) % 1000; }
        }

        public int Pins { get; set; } = 10;
        public int teeth_length { get; set; }

        public Gear(double x, double y, double r = 250, int p = 0)
        {
            Center = new Point(x, y);
            Radius = r;
            Phase = p;
            teeth_length = (int)(Radius / 10);
        }

        public virtual IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();

            var teeth_width = Math.PI * (Radius) / Pins;
            for (int i = 0; i < Pins; i++)
            {
                var tran = new TranslateTransform(Center.X, Center.Y - teeth_length / 2);
                var transform = new RotateTransform(i * 360.0 / Pins + Phase * 360.0 / 1000.0, Center.X, Center.Y);
                TransformGroup t = new TransformGroup
                {
                    Children = new TransformCollection(new Transform[] { tran, transform })
                };
                var rect = new Rectangle() { Width = teeth_width, Height = teeth_length, Fill = Brushes.Purple, RenderTransform = t };
                DrawingElements.Add(rect);
            }

            var gearBase = new Ellipse() { Width = Radius / 2, Height = Radius / 2, Fill = Brushes.Gray, Margin = new Thickness(Center.X - Radius / 4, Center.Y - Radius / 4, 0, 0) };
            DrawingElements.Add(gearBase);

            return DrawingElements;
        }

        public List<IConnection> Connections { get; set; }
    }
}