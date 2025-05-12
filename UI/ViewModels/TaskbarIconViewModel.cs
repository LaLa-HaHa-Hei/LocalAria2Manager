using System.Windows;
using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UI.Views;

namespace UI.ViewModels
{
    public partial class TaskbarIconViewModel : ObservableObject
    {
        private readonly IAria2SettingsService _aria2SettingsService;
        private readonly IUiSettingsService _uiSettingsService;
        private readonly Aria2ProcessService _aria2ProcessService;
        private readonly IWindowBoundsService _windowBoundsService;
        private AriaNgWindow? _ariaNgWindow = null;
        private Aria2LogWindow? _aria2LogWindow = null;
        private SettingsWindow? _settingsWindow = null;

        public TaskbarIconViewModel(IAria2SettingsService aria2SettingsService, IUiSettingsService uiSettingsService, IWindowBoundsService windowBoundsService, Aria2ProcessService aria2ProcessService)
        {
            _aria2SettingsService = aria2SettingsService;
            _uiSettingsService = uiSettingsService;
            _aria2ProcessService = aria2ProcessService;
            _windowBoundsService = windowBoundsService;

            try
            {
                _aria2ProcessService.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ExitApplication();
            }

            UiSettingsDto uiSettingsDto = _uiSettingsService.GetUiSettings();
            if (uiSettingsDto.ShowOnStartup)
            {
                ShowAriaNgWindow();
            }
        }

        [RelayCommand]
        public void OpenSettingsWindow()
        {
            if (_settingsWindow == null)
            {
                _settingsWindow = new(_aria2SettingsService, _uiSettingsService);
                _settingsWindow.Closed += (s, e) => _settingsWindow = null;
                _settingsWindow.Show();
            }
            else
            {
                _settingsWindow.Activate();
            }
        }
        // Aria2进程
        [RelayCommand]
        public void StartAria2() => _aria2ProcessService.Start();
        [RelayCommand]
        public async Task StopAria2() => await _aria2ProcessService.StopAsync();
        [RelayCommand]
        public async Task RestartAria2()
        {
            await _aria2ProcessService.StopAsync();
            StartAria2();
        }

        [RelayCommand]
        public void OpenAria2LogWindow()
        {
            if (_aria2LogWindow == null)
            {
                _aria2LogWindow = new(_aria2ProcessService);
                _aria2LogWindow.Closed += (s, e) => _aria2LogWindow = null;
                _aria2LogWindow.Show();
            }
            else
            {
                _aria2LogWindow.Activate();
            }
        }

        // AriaNgWindow
        [RelayCommand]
        public void ShowAriaNgWindow()
        {
            if (_ariaNgWindow == null)
            {
                _ariaNgWindow = new(_windowBoundsService);
                _ariaNgWindow.Closed += (s, e) => _ariaNgWindow = null;
                _ariaNgWindow.Show();
            }
            else if (_ariaNgWindow.IsVisible == false)
                _ariaNgWindow.Show();
            else
                _ariaNgWindow.Activate();
        }

        [RelayCommand]
        public void HideAriaNgWindow() => _ariaNgWindow?.Hide();

        [RelayCommand]
        public void ToggleWindowVisibility()
        {
            if (_ariaNgWindow?.IsVisible == true)
                HideAriaNgWindow();
            else
                ShowAriaNgWindow();
        }

        [RelayCommand]
        public void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
