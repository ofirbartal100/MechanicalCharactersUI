using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MechanicalCharacters.ViewModels;
using Newtonsoft.Json;

namespace MechanicalCharacters.Utils
{

    public static class AssemblySolver
    {
        public static JsonForAssemblies SolveForAssembly(GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs args)
        {
            //run python and get json
            Debug.Print($"Generating Assembly with Curve #{args}");
            //add parameters here
            // string json = RunPythonScript("C:\\Users\\ofir\\Desktop\\random_curve_generator.py");
            Debug.Print($"Finished Generating Assembly Curve #{args}");

            //parse json into a Curve
            //JsonForAssemblies jfassemblies = JsonConvert.DeserializeObject<JsonForAssemblies>(json);
            JsonForAssemblies jfassemblies = null;
            return jfassemblies;
        }


        private static string RunPythonScript(string cmd)
        {
            var pythonPath = "C:\\Users\\ofir\\Anaconda3\\python.exe";
            if (!File.Exists(pythonPath))
            {
                MessageBox.Show("No python file found at: " + pythonPath);
                return "";
            }

            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                FileName = pythonPath,
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
