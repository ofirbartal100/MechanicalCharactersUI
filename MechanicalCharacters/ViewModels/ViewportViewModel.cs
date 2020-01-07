using MechanicalCharacters.Utils;
using MechanicalCharacters.Utils.Components;
using Newtonsoft.Json;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MechanicalCharacters.ViewModels
{
    public class GenerateAssemblyToFitCurveEvent : PubSubEvent<GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs>
    {
        public class GenerateAssemblyToFitCurveEventArgs
        {
            public ConnectionInfo Info { get; set; }

            /// <summary>
            /// WRT Model Anchor
            /// </summary>
            public Point[] SampledeCurve { get; set; }
        }
    }

    public class SolvedAssemblyEvent : PubSubEvent<JsonForAssemblies> { }

    public class ToggleAnimationEvent : PubSubEvent<bool>
    {
    }

    public class ToggleEditCurveEvent : PubSubEvent<bool>
    {
    }

    public class ViewportViewModel : BindableBase
    {
        private readonly int counter = 0;
        private readonly IEventAggregator eventAggregator;
        private IEventAggregator _eventAggregator;
        private bool IsEndPointSelected = false;
        private bool IsInEditCurveMode = false;
        private DispatcherTimer t = new DispatcherTimer();

        public ViewportViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            UserInputs = new List<Curve>() { new Curve() };
            CanvasElements = new ObservableCollection<UIElement>();
            eventAggregator.GetEvent<ToggleAnimationEvent>().Subscribe(ToggleAnimationEventHandler);
            eventAggregator.GetEvent<ToggleEditCurveEvent>().Subscribe(ToggleEditCurveEventHandler);
            eventAggregator.GetEvent<GenerateAssemblyEvent>().Subscribe(GenerateAssemblyEventHandler);

            //debug
            GeneratedAssembly = new Assembly()
            {
                Components = new List<IComponent>()
                {
                    new Gear(100, 100),
                    new Stick(new StickConfiguration()
                    {
                        Anchor = new Point(50, 50),
                        Length = 50,
                        Length2 = 10,
                        Rotation = 50
                    })
                }
            };

            Model = new ArticulatedModel("");
            PinConnection.Connect(GeneratedAssembly.Components[0], GeneratedAssembly.Components[1], new Point(45, 45), new Point(115, 115));

            //GeneratedAssembly.LoadConfiguration();

            List<JsonConfigurationForArticulatedCharacter> configurations = LoadAll();
            t.Tick += (sender, args) =>
            {
                //Model.ApplyConfiguration(configurations[(counter++) % configurations.Count].Degrees.ToArray());
                //Model.Apply(modelConfig);
                //GeneratedAssembly.Apply(assemblyConfig);
                Render();
            };

            eventAggregator.GetEvent<SolvedAssemblyEvent>().Subscribe(SolvedAssemblyEventHandler);

            Render();
        }

        public ObservableCollection<UIElement> CanvasElements { get; set; }

        public Assembly GeneratedAssembly { get; set; }

        public ArticulatedModel Model { get; set; }

        public List<Curve> UserInputs { get; set; }

        public void AddPointCommand(Point position)
        {
            if (IsInEditCurveMode && IsEndPointSelected)
            {
                UserInputs[0].Add(position);
                Render();
            }
        }

        public void MouseLeftUpCommand(Point position)
        {
            if (CanvasElements.Any(element =>
                element is Ellipse &&
                ((Ellipse)element).Tag is ConnectionInfo &&
                ((ConnectionInfo)((Ellipse)element).Tag).IsSelected))
            {
                IsEndPointSelected = true;
            }

            if (IsInEditCurveMode && IsEndPointSelected)
            {
                AddPointCommand(position);
            }
        }

        public void RemovePointCommand(Point position)
        {
            if (IsInEditCurveMode && IsEndPointSelected)
            {
                UserInputs[0].RemoveAround(position);
                Render();
            }
        }

        private void GenerateAssemblyEventHandler()
        {
            Point[] anchorRelativeSampled = UserInputs[0].GetSampledCurve(Model.Anchor);
            ConnectionInfo info;
            if (IsEndPointSelected)
            {
                var target = (CanvasElements.First(element =>
                    element is Ellipse &&
                    ((Ellipse)element).Tag is ConnectionInfo &&
                    ((ConnectionInfo)((Ellipse)element).Tag).IsSelected)) as Ellipse;

                if (target == null) return;
                info = target?.Tag as ConnectionInfo;

                GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs args =
                    new GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs()
                    {
                        SampledeCurve = anchorRelativeSampled, Info = info
                    };
                _eventAggregator.GetEvent<GenerateAssemblyToFitCurveEvent>().Publish(args);

                //Assembly a = GenerateAssembly(sampled,target)
                //Renders
            }
        }

        private List<JsonConfigurationForArticulatedCharacter> LoadAll()
        {
            //load character from file
            //JsonForArticulatedCharacter o = new JsonForArticulatedCharacter
            //{
            //    AnchorPoint = new Point(500, 150),
            //    DegreesAndLengths = new List<Point>() { new Point(30, 100), new Point(100, 100), new Point(-100, 70) }
            //};
            //var json = JsonConvert.SerializeObject(o);
            //Debug.Print(json);

            //Model = new ArticulatedModel(json);

            ////animation
            //t.Interval = new TimeSpan(0, 0, 0, 0, 10);

            ////load configuration from file
            //JsonConfigurationForArticulatedCharacter jcfac = new JsonConfigurationForArticulatedCharacter() { Degrees = new List<double>() { 30, 40, -60, 20, 30, 40, -60, 20 } };
            //JsonConfigurationForArticulatedCharacter jcfac2 = new JsonConfigurationForArticulatedCharacter() { Degrees = new List<double>() { 30, -40, -60, 20, 310, -40, -160, 20 } };
            //JsonConfigurationForArticulatedCharacter jcfac3 = new JsonConfigurationForArticulatedCharacter() { Degrees = new List<double>() { 130, 40, -60, 130, 30, 40, -60, 120 } };
            //var json2 = JsonConvert.SerializeObject(new List<JsonConfigurationForArticulatedCharacter>() { jcfac, jcfac2, jcfac3 });
            //Debug.Print(json2);
            //var configurations = JsonConvert.DeserializeObject<List<JsonConfigurationForArticulatedCharacter>>(json2);

            return null;
        }

        private void Render()
        {
            CanvasElements.Clear();
            CanvasElements.AddRange(Model.GetDrawing());
            CanvasElements.AddRange(GeneratedAssembly.GetDrawing());
            //AssembliesDrawable.ForEach(assembly => CanvasElements.AddRange(assembly.GetDrawing()));

            UserInputs.ForEach(curve => CanvasElements.AddRange(curve.GetDrawing()));
        }

        private void SolvedAssemblyEventHandler(JsonForAssemblies obj)
        {
            //load from JsonForAssemblies
            //add to canvas
        }
        private void ToggleAnimationEventHandler(bool obj)
        {
            if (obj)
            {
                t.Start();
            }
            else
            {
                t.Stop();
            }
        }

        private void ToggleEditCurveEventHandler(bool obj)
        {
            IsInEditCurveMode = obj;
        }
    }
}