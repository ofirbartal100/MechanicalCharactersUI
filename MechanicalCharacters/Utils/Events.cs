namespace MechanicalCharacters.Utils
{
    public class
        GenerateAssemblyToFitCurveEvent : PubSubEvent<
            GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs>
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

    public class SolvedAssemblyEvent : PubSubEvent<AssemblySolver.CurveAssemblyAndAlignments>
    {
    }

    public class SolvedCurveEvent : PubSubEvent<List<Point>>
    {
    }

    public class ToggleAnimationEvent : PubSubEvent<bool>
    {
    }

    public class ToggleEditCurveEvent : PubSubEvent<bool>
    {
    }
}