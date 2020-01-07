using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils.Components
{
    public class StickConfiguration
    {
        public Point Anchor { get; set; }
        public double Length { get; set; }
        public double Length2 { get; set; }
        public double Rotation { get; set; }
    }
    public class Stick : IDrawable , IComponent
    {
        public static readonly SolidColorBrush StickColor = Brushes.LightCoral;
        public StickConfiguration Configuration { get; set; }

        public Stick(StickConfiguration config)
        {
            Configuration = config;
        }
        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();

            var translate = new TranslateTransform(Configuration.Anchor.X - Configuration.Length / 2, Configuration.Anchor.Y - Configuration.Length2 /2);
            var rotate = new RotateTransform(Configuration.Rotation, Configuration.Anchor.X, Configuration.Anchor.Y);
            TransformGroup tg = new TransformGroup
            {
                Children = new TransformCollection(new Transform[] { translate, rotate })
            };
            var rect = new Rectangle() { Width = Configuration.Length, Height = Configuration.Length2, Fill = StickColor, RenderTransform = tg };
            DrawingElements.Add(rect);

            return DrawingElements;
        }

        public List<IConnection> Connections { get; set; }
    }
}
