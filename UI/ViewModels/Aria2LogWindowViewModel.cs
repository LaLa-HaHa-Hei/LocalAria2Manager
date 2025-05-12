using System.Windows.Threading;
using BLL.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UI.ViewModels
{
    public partial class Aria2LogWindowViewModel : ObservableObject
    {
        private readonly Aria2ProcessService _aria2ProcessService;
        private readonly DispatcherTimer _updateLogTimer = new();
        [ObservableProperty]
        private string _outputLog = string.Empty;
        [ObservableProperty]
        private string _errorLog = string.Empty;

        public Aria2LogWindowViewModel(Aria2ProcessService aria2ProcessService)
        {
            _aria2ProcessService = aria2ProcessService;
            _updateLogTimer.Interval = TimeSpan.FromSeconds(1);
            _updateLogTimer.Tick += (s, e) =>
            {
                OutputLog = _aria2ProcessService.GetOutputLog();
                ErrorLog = _aria2ProcessService.GetErrorLog();
            };
            _updateLogTimer.Start();
        }
    }
}
