using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using BLL.DTOs;


namespace UI.ViewModels
{
    public class SettingsWindowViewModel : ObservableObject
    {
        private readonly IAria2SettingsService _aria2SettingsService;
        private readonly IUiSettingsService _uiSettingsService;
        private Aria2SettingsDto _aria2Settings;
        private UiSettingsDto _uiSettings;

        private bool _shouldClose = false;
        private string _directoryPath = string.Empty;
        private int _rpcListenPort;
        private bool _showOnStartup;

        public bool ShouldClose
        {
            get => _shouldClose;
            set => SetProperty(ref _shouldClose, value);
        }
        public string DirectoryPath
        {
            get => _directoryPath;
            set => SetProperty(ref _directoryPath, value);
        }
        public int RpcListenPort
        {
            get => _rpcListenPort;
            set => SetProperty(ref _rpcListenPort, value);
        }
        public bool ShowOnStartup
        { 
            get => _showOnStartup;
            set => SetProperty(ref _showOnStartup, value);
        }

        public RelayCommand SaveSettingsCommand { get; }

        public SettingsWindowViewModel(IAria2SettingsService aria2SettingsService, IUiSettingsService uiSettingsService)
        {
            _aria2SettingsService = aria2SettingsService;
            _uiSettingsService = uiSettingsService;
            _aria2Settings = _aria2SettingsService.GetAria2Settings();
            _uiSettings = _uiSettingsService.GetUiSettings();
            DirectoryPath = _aria2Settings.DirectoryPath;
            RpcListenPort = _aria2Settings.RpcListenPort;
            ShowOnStartup = _uiSettings.ShowOnStartup;
            SaveSettingsCommand = new(() => 
            {
                _aria2SettingsService.SaveAria2Settings(new Aria2SettingsDto
                {
                    DirectoryPath = DirectoryPath,
                    RpcListenPort = RpcListenPort
                });
                _uiSettingsService.SaveUiSettings(new UiSettingsDto
                {
                    ShowOnStartup = ShowOnStartup
                });
                ShouldClose = true;
            });
        }
    }
}
