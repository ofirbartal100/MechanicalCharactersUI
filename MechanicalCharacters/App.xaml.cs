using System;
using MechanicalCharacters.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;

namespace MechanicalCharacters
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register(typeof(Object), typeof(Workspace), "Workspace");
            containerRegistry.Register(typeof(Object), typeof(Viewport), "Viewport");
            containerRegistry.Register(typeof(Object), typeof(Controls), "Controls");
        }
    }
}
