using Jot;
using Jot.Storage;
using System;

namespace MechanicalCharacters.Utils
{
    // Expose services as static class to keep the example simple
    public static class Services
    {
        // expose the tracker instance
        public static Tracker Tracker = new Tracker(new JsonFileStore(Environment.CurrentDirectory));

        static Services()
        {
            // tell Jot how to track Window objects
            Tracker.Configure<PythonPaths>()
                .Id(d => "PythonPaths").Properties(d => new { d.PythonPath, d.PythonScriptPath });
        }

        public class PythonPaths
        {
            public PythonPaths()
            {
            }

            public string PythonPath { get; set; }
            public string PythonScriptPath { get; set; }
        }
    }
}