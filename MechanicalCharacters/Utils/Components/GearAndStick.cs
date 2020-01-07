using System;
using System.Collections.Generic;
using System.Windows;

namespace MechanicalCharacters.Utils.Components
{
    internal class GearAndStick : IDrawable
    {
        public Gear Gear { get; set; }
        public StickConnection Stick { get; set; }
        public double DistanceFromCenter { get; set; }

        public GearAndStick(double x, double y, double r, double d)
        {
            Gear = new Gear(x, y) { Radius = r };

            d = d < r ? (d > 0 ? d : 1) : r - 1;
            DistanceFromCenter = d;
            Stick = new StickConnection(x + DistanceFromCenter, y);
        }

        public void Step(int step = 1)
        {
            Gear.Phase += step;

            var connectionPoint = new Point(Gear.Center.X + DistanceFromCenter * Math.Cos(Gear.Phase / 1000.0 * 2 * Math.PI),
                Gear.Center.Y + DistanceFromCenter * Math.Sin(Gear.Phase / 1000.0 * 2 * Math.PI));
            Stick.Anchor = connectionPoint;
        }

        internal void ApplyConfiguration(double deg)
        {
            Stick.RotationDegree = deg;
        }

        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();
            DrawingElements.AddRange(Gear.GetDrawing());
            DrawingElements.AddRange(Stick.GetDrawing());
            return DrawingElements;
        }
    }
}