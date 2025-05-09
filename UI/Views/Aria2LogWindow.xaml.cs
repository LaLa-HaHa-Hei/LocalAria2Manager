using System.Windows;
using BLL.Services;
using UI.ViewModels;

namespace UI.Views
{
    /// <summary>
    /// Aria2LogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class Aria2LogWindow : Window
    {
        public Aria2LogWindow(Aria2ProcessService aria2ProcessService)
        {
            InitializeComponent();
            DataContext = new Aria2LogWindowViewModel(aria2ProcessService);
        }
    }
}
