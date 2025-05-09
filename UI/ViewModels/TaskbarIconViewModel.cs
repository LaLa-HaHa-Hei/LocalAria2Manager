using System.Windows;
using BLL.DTOs;
using BLL.Interfaces;
using BLL.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UI.Views;

namespace UI.ViewModels
{
    public class TaskbarIconViewModel : ObservableObject
    {
        private readonly IAria2SettingsService _aria2SettingsService;
        private readonly IUiSettingsService _uiSettingsService;
        private readonly Aria2ProcessService _aria2ProcessService;
        private readonly IWindowBoundsService _windowBoundsService;
        private AriaNgWindow? _ariaNgWindow = null;
        private Aria2LogWindow? _aria2LogWindow = null;
        private SettingsWindow? _settingsWindow = null;

        public RelayCommand OpenSettingsWindowCommand { get; }
        public RelayCommand RestartAria2Command { get; }
        public RelayCommand StartAria2Command { get; }
        public RelayCommand StopAria2Command { get; }
        public RelayCommand OpenAria2LogWindowCommand { get; }
        public RelayCommand ShowWindowCommand { get; }
        public RelayCommand HideWindowCommand { get; }
        public RelayCommand ToggleWindowVisibilityCommand { get; }
        public RelayCommand ExitApplicationCommand { get; }

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

            OpenSettingsWindowCommand = new(() =>
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
            });
            StartAria2Command = new(() => _aria2ProcessService.Start());
            StopAria2Command = new(async () => await Task.Run(() => _aria2ProcessService.StopAsync()));
            RestartAria2Command = new(async () =>
            {
                await Task.Run(async () =>
                {
                    await _aria2ProcessService.StopAsync();
                    _aria2ProcessService.Start();
                });
            });
            OpenAria2LogWindowCommand = new(() =>
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
            });
            ShowWindowCommand = new(() =>
            {
                if (_ariaNgWindow == null)
                {
                    _ariaNgWindow = new(_windowBoundsService);
                    _ariaNgWindow.Closed += (s, e) => _ariaNgWindow = null;
                    _ariaNgWindow.Show();
                }
                else
                {
                    _ariaNgWindow.Show();
                    _ariaNgWindow.Activate();
                }
            });
            HideWindowCommand = new(() => _ariaNgWindow?.Hide());
            ToggleWindowVisibilityCommand = new(() =>
            {
                if (_ariaNgWindow?.IsVisible == true)
                    HideWindowCommand.Execute(null);
                else
                    ShowWindowCommand.Execute(null);
            });
            ExitApplicationCommand = new RelayCommand(ExitApplication);


            UiSettingsDto uiSettingsDto = _uiSettingsService.GetUiSettings();
            if (uiSettingsDto.ShowOnStartup)
            {
                ShowWindowCommand.Execute(null);
            }
        }

        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }
    }
}
