using MechanicalCharacters.Utils;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;

namespace MechanicalCharacters.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Computational Design Of Mechanical Characters";

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IEventAggregator _eventAggregator;
        public MainWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            var d = new Dictionary<string, string>
            {
                ["ContentRegion"] = "Workspace"
            };
            regionManager.Navigate(d);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<GenerateAssemblyToFitCurveEvent>().Subscribe(GenerateAssemblyToFitCurveEventHandler);
        }

        private void GenerateAssemblyToFitCurveEventHandler(GenerateAssemblyToFitCurveEvent.GenerateAssemblyToFitCurveEventArgs args)
        {
            var assembly = AssemblySolver.SolveForAssembly(args);
            _eventAggregator.GetEvent<SolvedAssemblyEvent>().Publish(assembly);
        }
    }
}