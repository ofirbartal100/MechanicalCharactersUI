using MechanicalCharacters.Utils;
using MechanicalCharacters.Utils.Components;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
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

    public class SolvedAssemblyEvent : PubSubEvent<AssemblySolver.CurveAssemblyAndAlignments> { }

    public class SolvedCurveEvent : PubSubEvent<List<Point>> { }

    public class ToggleAnimationEvent : PubSubEvent<bool>
    {
    }

    public class ToggleEditCurveEvent : PubSubEvent<bool>
    {
    }

    public class ClearEvent : PubSubEvent
    {
    }

    public class ViewportViewModel : BindableBase
    {
        private readonly int counter = 0;
        private readonly IEventAggregator eventAggregator;
        private IEventAggregator _eventAggregator;
        private bool IsEndPointSelected = true;
        private bool IsInEditCurveMode = true;
        private DispatcherTimer t = new DispatcherTimer();

        public ViewportViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            UserInputs = new List<Curve>() { new Curve() };
            CanvasElements = new ObservableCollection<UIElement>();
            eventAggregator.GetEvent<ClearEvent>().Subscribe(ClearEventHandler);
            eventAggregator.GetEvent<ToggleEditCurveEvent>().Subscribe(ToggleEditCurveEventHandler);
            eventAggregator.GetEvent<GenerateAssemblyEvent>().Subscribe(GenerateAssemblyEventHandler);

            //debug
            //GeneratedAssembly = new Assembly()
            //{
            //    Components = new List<IComponent>()
            //    {
            //        new Gear(300, 100),
            //        new Stick(new StickConfiguration()
            //        {
            //            Anchor = new Point(250, 50),
            //            Length = 50,
            //            Length2 = 10,
            //            Rotation = 50
            //        })
            //    }
            //};

            Model = new ArticulatedModel();
            //PinConnection.Connect(GeneratedAssembly.Components[0], GeneratedAssembly.Components[1], new Point(245, 45), new Point(315, 115));

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
        public List<Ellipse> CurvesElements { get; set; } = new List<Ellipse>();

        //public Assembly GeneratedAssembly { get; set; }

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

        private void ClearEventHandler()
        {
            UserInputs[0].PointsList.Clear();
            CurvesElements.Clear();
            Model = new ArticulatedModel();
            Render();
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
            Point[] anchorRelativeSampled = UserInputs[0].GetSampledCurve();
            ConnectionInfo info;
            if (IsEndPointSelected)
            {
                //var target = (CanvasElements.First(element =>
                //    element is Ellipse &&
                //    ((Ellipse)element).Tag is ConnectionInfo &&
                //    ((ConnectionInfo)((Ellipse)element).Tag).IsSelected)) as Ellipse;

                //if (target == null) return;
                //info = target?.Tag as ConnectionInfo;

                GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs args =
                    new GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs()
                    {
                        SampledeCurve = anchorRelativeSampled,
                        Info = null
                    };
                _eventAggregator.GetEvent<GenerateAssemblyToFitCurveEvent>().Publish(args);
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
            //CanvasElements.AddRange(GeneratedAssembly.GetDrawing());
            //AssembliesDrawable.ForEach(assembly => CanvasElements.AddRange(assembly.GetDrawing()));

            UserInputs.ForEach(curve => CanvasElements.AddRange(curve.GetDrawing()));
            CurvesElements.ForEach(curve => CanvasElements.Add(curve));


            //// fixes y axis

            //for (int i = 0; i < CanvasElements.Count; i++)
            //{
            //    CanvasElements[i].
            //}
        }

        private void SolvedAssemblyEventHandler(AssemblySolver.CurveAssemblyAndAlignments obj)
        {
            //transform curve with 0 mean
            var mean = obj.c_a.Curve.Points.Aggregate(new Point(), (m, p) =>
            {
                m.X += p.X;
                m.Y += p.Y;
                return m;
            });
            mean.X /= 72;
            mean.Y /= 72;
            double toflip = 1;//obj.ToFlip ? -1 : 1;
            var pts = obj.c_a.Curve.Points.Select(point =>
                {

                    var x = (point.X - mean.X);
                    var y = (point.Y - mean.Y)* toflip;

                    var rotatedX = (Math.Cos(-obj.RadRotation) * x -
                                   Math.Sin(-obj.RadRotation) * y) * obj.Scale;
                    var rotatedY = (Math.Cos(-obj.RadRotation) * y +
                                   Math.Sin(-obj.RadRotation) * x) * obj.Scale ;
                    return new Point(rotatedX + obj.MeanPoint.X, rotatedY + obj.MeanPoint.Y);
                }
            ).ToList();

            //plot
            CurvesElements.Clear();
            CurvesElements.AddRange(pts.Select(point =>
            {
                var tran2 = new TranslateTransform(point.X - 2, point.Y - 2);
                return new Ellipse() { Width = 4, Height = 4, Fill = Brushes.Green, RenderTransform = tran2 };
            }));
            Model= new ArticulatedModel();
            Model.Load(obj);
            //load from JsonForAssemblies
            //add to canvas

            Render();
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