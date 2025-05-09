using System.Diagnostics;
using System.Windows;
using BLL.Interfaces;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(IAria2SettingsService aria2SettingsService, IUiSettingsService uiSettingsService)
        {
            InitializeComponent();
            var vm = new SettingsWindowViewModel(aria2SettingsService, uiSettingsService);
            DataContext = vm;

            vm.PropertyChanged += (s, e) =>
            {
                Debug.WriteLine($"Property changed: {e.PropertyName}");
                if (e.PropertyName == nameof(SettingsWindowViewModel.ShouldClose) && vm.ShouldClose)
                {
                    Close();
                }
            };
        }
    }
}
