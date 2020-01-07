using MechanicalCharacters.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace MechanicalCharacters.Views
{
    /// <summary>
    /// Interaction logic for Viewport
    /// </summary>
    public partial class Viewport : UserControl
    {
        public Viewport()
        {
            InitializeComponent();
        }

        private void UIElement_OnMouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as Canvas);
            ((ViewportViewModel)this.DataContext).MouseLeftUpCommand(p);
        }

        private void UIElement_OnMouseRightUp(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(sender as Canvas);
            ((ViewportViewModel)this.DataContext).RemovePointCommand(p);
        }
    }
}