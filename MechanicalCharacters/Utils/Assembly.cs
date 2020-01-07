using MechanicalCharacters.Utils.Components;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils
{
    public interface IConfigurable
    {
        /// <summary>
        /// Loads a model from json string
        /// </summary>
        /// <param name="json"></param>
         void Load(string json);
        /// <summary>
        /// Applies a state to the model
        /// </summary>
         void Apply(string json);

    }
    public class Assembly : IDrawable , IConfigurable
    {
        public List<IComponent> Components { get; set; }

        public Assembly()
        {
            Components = new List<IComponent>();
        }


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
            throw new System.NotImplementedException();
        }

        public void Apply(string json)
        {
            throw new System.NotImplementedException();
        }
    }
}