using System.Windows.Threading;
using BLL.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UI.ViewModels
{
    public class Aria2LogWindowViewModel : ObservableObject
    {
        private readonly Aria2ProcessService _aria2ProcessService;
        private readonly DispatcherTimer _updateLogTimer = new();
        private string _outputLog = string.Empty;
        private string _errorLog = string.Empty;

        public string OutputLog
        {
            get => _outputLog;
            set => SetProperty(ref _outputLog, value);
        }
        public string ErrorLog
        {
            get => _errorLog;
            set => SetProperty(ref _errorLog, value);
        }

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
