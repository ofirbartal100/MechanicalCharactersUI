using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MechanicalCharacters.Utils
{
    public class JsonConfigurationForArticulatedCharacter
    {
        public List<double> Degrees { get; set; }
    }

    public class JsonForArticulatedCharacter
    {
        public Point AnchorPoint { get; set; }
        public List<Point> DegreesAndLengths { get; set; }
    }
    public class JsonForAssemblies
    {
        public List<double> Degrees { get; set; }
    }
}
