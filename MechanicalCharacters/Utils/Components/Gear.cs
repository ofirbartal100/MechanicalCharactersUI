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

        public double Alpha { get; set; }

        public int Pins { get; set; } = 10;
        public int teeth_length { get; set; }

        public Gear(double x, double y, double r = 50,double a=0, int p = 0)
        {
            Center = new Point(x, y);
            Radius = r;
            Alpha = a;
            Phase = p;
            teeth_length = (int)(Radius / 10);
        }

        public virtual IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();

            var teeth_width = Math.PI * (Radius) / Pins;
            for (int i = 0; i < Pins; i++)
            {
                var tran = new TranslateTransform(Center.X- teeth_width/2, Center.Y - Radius - teeth_length / 2);
                //var transform = new RotateTransform(i * 360.0 / Pins + Phase * 360.0 / 1000.0, Center.X, Center.Y);
                var transform = new RotateTransform(i * 360.0 / Pins + Alpha, Center.X , Center.Y );
                TransformGroup t = new TransformGroup
                {
                    Children = new TransformCollection(new Transform[] { tran, transform })
                };
                var rect = new Rectangle() { Width = teeth_width, Height = teeth_length, Fill = Brushes.Purple, RenderTransform = t };
                DrawingElements.Add(rect);
            }
            var tran2 = new TranslateTransform(Center.X-Radius, Center.Y - Radius);
            var gearBase = new Ellipse() { Width = Radius*2, Height = Radius*2, Fill = Brushes.Gray,RenderTransform = tran2 };
            DrawingElements.Add(gearBase);

            return DrawingElements;
        }

        public List<IConnection> Connections { get; set; }
    }
}