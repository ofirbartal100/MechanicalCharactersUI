using System.Collections.Generic;
using System.Windows;

namespace MechanicalCharacters.Utils
{
    public interface IDrawable
    {
        IEnumerable<UIElement> GetDrawing();
    }
}