using MechanicalCharacters.Utils.Components;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;

namespace MechanicalCharacters.Utils
{
    public class ArticulatedModel : IDrawable, IConfigurable
    {
        public ArticulatedModel(string json = null)
        {
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
            var forArticulatedCharacter = JsonConvert.DeserializeObject<List<ParseObject>>(json);
            //ArticulatedFigure = new StickConnection(forArticulatedCharacter.AnchorPoint.X, forArticulatedCharacter.AnchorPoint.Y, forArticulatedCharacter.DegreesAndLengths[0].X, forArticulatedCharacter.DegreesAndLengths[0].Y);
            //for (int i = 1; i < forArticulatedCharacter.DegreesAndLengths.Count; i++)
            //{
            //    ArticulatedFigure.AddLastChild(forArticulatedCharacter.DegreesAndLengths[i].X, forArticulatedCharacter.DegreesAndLengths[i].Y);
            //}
            double meanX = forArticulatedCharacter.Sum(o => o.position[0]) / forArticulatedCharacter.Count;
            double meanY = forArticulatedCharacter.Sum(o => o.position[1]) / forArticulatedCharacter.Count;
            meanX = 0;
            meanY = 0;
            double scale = 10;
            var forArticulatedCharacterAlined = forArticulatedCharacter.Select(o =>
            {
                o.position[0] -= meanX;
                o.position[1] -= meanY;

                o.position[0] *= scale;
                o.position[1] *= scale;
                o.length *= scale;

                o.position[0] += meanX;
                o.position[1] += meanY;

                return o;
            });

            foreach (var parseObject in forArticulatedCharacterAlined)
            {
                double centerScreenWidth = 1920 / 4;
                double centerScreenHeight = 1080 / 4;
                switch (parseObject.type)
                {
                    case "Gear":
                        Components.Add(new Gear(centerScreenWidth + parseObject.position[0], centerScreenHeight + parseObject.position[1], parseObject.length, parseObject.orientation[2]));
                        break;

                    case "Stick":
                        StickConfiguration config = new StickConfiguration()
                        {
                            Length = parseObject.length,
                            Length2 = 5,
                            Rotation = 360 - parseObject.orientation[2]
                        };

                        config.Anchor = new Point(
                            centerScreenWidth + ((parseObject.position[0])),
                            centerScreenHeight + ((parseObject.position[1])));

                        Components.Add(new Stick(config));
                        break;

                    default:
                        break;
                }
            }
        }

        public void Load(AssemblySolver.CurveAssemblyAndAlignments ass)
        {
            double xOffset = 800 / 2;
            double yOffset = 450 / 2;

            xOffset = Math.Abs(ass.c_a.Assembly.Components.Min(o =>
            {
                if (o.type.Contains("Gear"))
                {
                    return (o.position[0] - o.length) * 10;
                }
                else if (o.type.Contains("Stick"))
                {
                    return (o.position[0] + o.length * Math.Cos(o.orientation[2] * Math.PI / 180)) * 10;
                }
                return 9999;
            })) + 5;

            yOffset = Math.Abs(ass.c_a.Assembly.Components.Min(o =>
            {
                if (o.type.Contains("Gear"))
                {
                    return (o.position[1] - o.length) * 10;
                }
                else if (o.type.Contains("Stick"))
                {
                    return (o.position[1] + o.length * Math.Sin(o.orientation[2] * Math.PI / 180)) * 10;
                }
                return 9999;
            })) + 5;
            double toflip = ass.ToFlip ? -1 : 1;
            var forArticulatedCharacterAlined = ass.c_a.Assembly.Components.Select(o =>
            {
                //var x = (o.position[0]);
                //var y = (o.position[1])*toflip;

                //o.position[0] = (Math.Cos(-ass.RadRotation) * x -
                //                Math.Sin(-ass.RadRotation) * y) * ass.Scale + centerScreenWidth;
                //o.position[1] = (Math.Cos(-ass.RadRotation) * y +
                //               Math.Sin(-ass.RadRotation) * x) * ass.Scale + centerScreenHeight;
                o.position[0] = o.position[0] * 10 + xOffset;
                o.position[1] = o.position[1] * 10 + yOffset;

                o.length *= 10;
                //o.length *= ass.Scale;
                //o.orientation[2] = (360 + o.orientation[2] - ass.RadRotation * 180 / Math.PI) % 360;
                return o;
            });

            //centerScreenWidth = 0;
            //centerScreenHeight = 0;
            foreach (var parseObject in forArticulatedCharacterAlined)
            {
                switch (parseObject.type)
                {
                    case "Gear":
                        Components.Add(new Gear(parseObject.position[0], parseObject.position[1], parseObject.length, parseObject.orientation[2]));
                        break;

                    case "Stick":
                        StickConfiguration config = new StickConfiguration()
                        {
                            Length = parseObject.length,
                            Length2 = 1,
                            Rotation = parseObject.orientation[2]
                        };

                        config.Anchor = new Point(((parseObject.position[0])), ((parseObject.position[1])));

                        Components.Add(new Stick(config));
                        break;

                    default:
                        break;
                }
            }
        }

        public void Apply(string json)
        {
            //var configurationForArticulatedCharacter = JsonConvert.DeserializeObject<JsonConfigurationForArticulatedCharacter>(json);
            //ApplyConfiguration(configurationForArticulatedCharacter.Degrees.ToArray());
        }
    }

    public class ParseObject
    {
        public string type { get; set; }
        public int id { get; set; }
        public double length { get; set; }
        public List<double> position { get; set; }
        public List<double> orientation { get; set; }
    }
}