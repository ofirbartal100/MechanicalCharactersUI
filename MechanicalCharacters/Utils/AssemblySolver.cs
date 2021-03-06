﻿using MechanicalCharacters.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace MechanicalCharacters.Utils
{
    public static class AssemblySolver
    {
        private static readonly object key = new object();
        private static readonly int count = 0;
        public static Services.PythonPaths Paths { get; set; } = new Services.PythonPaths();

        static AssemblySolver()
        {
            lock (key)
            {
                Services.Tracker.Track(Paths);
                if (string.IsNullOrEmpty(Paths.PythonPath))
                {
                    Paths.PythonPath = "No Path Given";
                    Services.Tracker.Persist(Paths);
                }
                if (string.IsNullOrEmpty(Paths.PythonScriptPath))
                {
                    Paths.PythonScriptPath = "No Path Given";
                    Services.Tracker.Persist(Paths);
                }
                if (string.IsNullOrEmpty(Paths.DBPath))
                {
                    Paths.DBPath = "No Path Given";
                    Services.Tracker.Persist(Paths);
                }

                if (!File.Exists(Paths.PythonPath))
                {
                    MessageBox.Show("No valid PythonPath found at: \"" + Paths.PythonPath + "\" \nPlease Change In The Configuration File At: " + Environment.CurrentDirectory);
                    Environment.Exit(1);
                }
                if (!File.Exists(Paths.PythonScriptPath))
                {
                    MessageBox.Show("No valid PythonScriptPath found at: \"" + Paths.PythonScriptPath + "\" \nPlease Change In The Configuration File At: " + Environment.CurrentDirectory);
                    Environment.Exit(1);
                }
                if (!File.Exists(Paths.DBPath))
                {
                    MessageBox.Show("No valid DBPath found at: \"" + Paths.DBPath + "\" \nPlease Change In The Configuration File At: " + Environment.CurrentDirectory);
                    Environment.Exit(1);
                }
            }
        }

        public static CurveAssemblyAndAlignments SolveForAssembly(GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs args)
        {
            //run python and get json

            string json_path = Path.Combine(Environment.CurrentDirectory, "user_defined_curve.json");
            var meanX = args.SampledeCurve.Sum(point => point.X) / args.SampledeCurve.Length;
            var meanY = args.SampledeCurve.Sum(point => point.Y) / args.SampledeCurve.Length;
            var pts = args.SampledeCurve.Select(point => new List<double>() { point.X - meanX, point.Y - meanY, 0 }).ToList();
            var max = pts.Max(list => list.Max(d => Math.Abs(d)));

            var pts2 = pts.Select(list => new List<double>() { list[0], list[1], 0 }).ToList();

            var data = new double[72, 3];
            for (int i = 0; i < 72; i++)
            {
                data[i, 0] = pts2[i][0];
                data[i, 1] = pts2[i][1];
                data[i, 2] = pts2[i][2];
            }
            double[] s2 = new double[2];
            var v = new double[3, 2];
            alglib.pcatruncatedsubspace(data, 72, 3, 2, 0.001, 500, out s2, out v);
            double rad_angleOfMaxVarianceAxisFromX = Math.Acos(v[0, 0]);
            bool toFlip = (v[0, 0] * v[1, 1] - v[1, 0] * v[0, 1]) < 0;

            List<List<double>> pts4 = new List<List<double>>();
            foreach (var ppp in pts2)
            {
                pts4.Add(new List<double>() {(ppp[0] * v[0, 0] + ppp[1] * v[1, 0] + ppp[2] * v[2, 0]),
                (ppp[0] * v[0, 1] + ppp[1] * v[1, 1] + ppp[2] * v[2, 1]),0});
            }

            max = pts4.Max(list => list.Max(d => Math.Abs(d)));

            string json_object = JsonConvert.SerializeObject(pts2);
            System.IO.File.WriteAllText(json_path, json_object);
            //add parameters here
            string json = RunPythonScript("\"" + Paths.PythonScriptPath + "\" -json_path \"" + json_path + "\" -db_path \"" + Paths.DBPath + "\"");

            JsonOfCurveAndAssembly c_a = JsonConvert.DeserializeObject<JsonOfCurveAndAssembly>(json);

            var maxOfResult = c_a.Curve.Points.Max(point => Math.Abs(point.X));

            //parse json into a Curve
            CurveAssemblyAndAlignments caa = new CurveAssemblyAndAlignments()
            {
                c_a = c_a,
                MeanPoint = new Point(meanX, meanY),
                Scale = max / maxOfResult,
                RadRotation = rad_angleOfMaxVarianceAxisFromX,
                ToFlip = toFlip
            };
            return caa;
        }

        public class CurveAssemblyAndAlignments
        {
            public JsonOfCurveAndAssembly c_a { get; set; }
            public double Scale { get; set; }
            public double RadRotation { get; set; }
            public Point MeanPoint { get; set; }
            public bool ToFlip { get; set; }
        }

        private static string RunPythonScript(string cmd)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = Paths.PythonPath,
                //Arguments = string.Format("\"{0}\" \"{1}\""),

                //StackOverflow:
                //When standard output it was being redirected,
                //the event in C# wasn't being raised when a line was written on console because there were no calls to stdout.flush;
                //Putting a stdout.flush() statement after each print statement made the events fire as they should and C# now captures the output as it comes.
                //Or you could just use -u switch.
                Arguments = "-u " + cmd,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(StartInfo))
            {
                using (StreamReader reader = process.StandardOutput)
                {
                    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                    return result;
                }
            }
        }
    }
}