using MechanicalCharacters.Utils.Components;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils
{
    public class ArticulatedModel : IDrawable, IConfigurable
    {
        public ArticulatedModel(string json)
        {
            Load(json);
        }
        public List<IComponent> Components { get; set; } = new List<IComponent>();

        public Point Anchor { get; set; }
        //public StickConnection ArticulatedFigure { get; set; }

        //we can apply a certain configuration with this function
        //public void ApplyConfiguration(double[] degrees)
        //{
        //    //var cur = Components;
        //    //foreach (var degree in degrees)
        //    //{
        //    //    if (cur == null) break;
        //    //    cur.SetRotation(degree);
        //    //    cur = cur.ChildConnection;
        //    //}
        //}

        public IEnumerable<UIElement> GetDrawing()
        {
            var DrawingElements = new List<UIElement>();
            foreach (var component in Components)
            {
                DrawingElements.AddRange(component.GetDrawing());
                if (component.Connections == null) continue;
                foreach (var connection in component.Connections)
                {
                    DrawingElements.AddRange(connection.GetDrawing());
                }
            }
            DrawingElements.Sort((element, uiElement) =>
            {
                if (element is Ellipse && uiElement is Ellipse) return 0;
                else if (element is Ellipse && !(uiElement is Ellipse)) return 1;
                else if (uiElement is Ellipse && !(element is Ellipse)) return -1;
                else return 0;
            });
            return DrawingElements;
        }

        public void Load(string json)
        {
            //var forArticulatedCharacter = JsonConvert.DeserializeObject<JsonForArticulatedCharacter>(json);
            //ArticulatedFigure = new StickConnection(forArticulatedCharacter.AnchorPoint.X, forArticulatedCharacter.AnchorPoint.Y, forArticulatedCharacter.DegreesAndLengths[0].X, forArticulatedCharacter.DegreesAndLengths[0].Y);
            //for (int i = 1; i < forArticulatedCharacter.DegreesAndLengths.Count; i++)
            //{
            //    ArticulatedFigure.AddLastChild(forArticulatedCharacter.DegreesAndLengths[i].X, forArticulatedCharacter.DegreesAndLengths[i].Y);
            //}
        }

        public void Apply(string json)
        {
            //var configurationForArticulatedCharacter = JsonConvert.DeserializeObject<JsonConfigurationForArticulatedCharacter>(json);
            //ApplyConfiguration(configurationForArticulatedCharacter.Degrees.ToArray());
        }
    }
}