using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Prism.Events;

namespace MechanicalCharacters.ViewModels
{
    public class GenerateAssemblyEvent : PubSubEvent
    {
        
    }
    public class ControlsViewModel : BindableBase
    {
        public ICommand ToggleAnimationClickCommand { get; set; }
        public ICommand ToggleEditCurveClickCommand { get; set; }
        public ICommand GenerateAssemblyClickCommand { get; set; }

        private IEventAggregator _eventAggregator;
        public ControlsViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            ToggleAnimationClickCommand = new DelegateCommand<object>(ToggleAnimationClick);
            ToggleEditCurveClickCommand = new DelegateCommand<object>(ToggleEditCurveClick);
            GenerateAssemblyClickCommand = new DelegateCommand(GenerateAssemblyClick);
        }

        private void GenerateAssemblyClick()
        {
            _eventAggregator.GetEvent<GenerateAssemblyEvent>().Publish();
        }

        private void ToggleAnimationClick(object obj)
        {
            var b = (bool) obj;
            _eventAggregator.GetEvent<ToggleAnimationEvent>().Publish(b);
        }
        private void ToggleEditCurveClick(object obj)
        {
            var b = (bool) obj;
            _eventAggregator.GetEvent<ToggleEditCurveEvent>().Publish(b);
        }
    }
}
