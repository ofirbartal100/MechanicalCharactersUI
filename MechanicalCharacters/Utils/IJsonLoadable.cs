using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechanicalCharacters.Utils
{
    interface IJsonLoadable
    {
        void LoadFromJson(string json);
    }
}
