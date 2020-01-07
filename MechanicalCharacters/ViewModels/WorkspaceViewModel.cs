using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using MechanicalCharacters.Utils;
using Prism.Regions;

namespace MechanicalCharacters.ViewModels
{
    public class WorkspaceViewModel : BindableBase
    {
        public WorkspaceViewModel(IRegionManager regionManager)
        {
            var d = new Dictionary<string, string>
            {
                ["VP"] = "Viewport",
                ["CT"] = "Controls"
            };
            regionManager.Navigate(d);
        }
    }
}
