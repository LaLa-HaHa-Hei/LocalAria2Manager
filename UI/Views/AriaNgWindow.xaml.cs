using System.IO;
using System.Windows;
using BLL.DTOs;
using BLL.Interfaces;

namespace UI.Views
{
    /// <summary>
    /// AriaNgWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AriaNgWindow : Window
    {
        private readonly string _htmlFilePath;
        private readonly string _htmlUri = null!;
        private readonly IWindowBoundsService _windowBoundsService;
        private bool _isWebViewInitialized = false;

        public AriaNgWindow(IWindowBoundsService windowBoundsService)
        {
            InitializeComponent();

            _windowBoundsService = windowBoundsService;
            _htmlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AriaNg", "index.html");
            // 获取本地html文件路径
            if (File.Exists(_htmlFilePath))
            {
                _htmlUri = new Uri(_htmlFilePath).AbsoluteUri;
            }
            else
            {
                MessageBox.Show("AriaNg的HTML文件不存在: " + _htmlFilePath);
                throw new FileNotFoundException($"{_htmlFilePath} not found", _htmlFilePath);
            }
            WindowBoundsDto windowBoundsDto = _windowBoundsService.GetWindowBounds();
            Width = windowBoundsDto.Width;
            Height = windowBoundsDto.Height;
            Left = windowBoundsDto.Left;
            Top = windowBoundsDto.Top;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            // 确保WebView2初始化
            await AriaNgWebView2.EnsureCoreWebView2Async();
            _isWebViewInitialized = true; // 初始化完成
            AriaNgWebView2.CoreWebView2.Navigate(_htmlUri);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _windowBoundsService.SaveWindowBounds(new WindowBoundsDto
            {
                Width = Width,
                Height = Height,
                Left = Left,
                Top = Top
            });
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_isWebViewInitialized) return; // 防止初始化没完成就执行

            if (IsVisible)
            {
                AriaNgWebView2.CoreWebView2.Navigate(_htmlUri);
            }
            else
            {
                AriaNgWebView2.CoreWebView2.NavigateToString("<html></html>");
            }
        }
    }
}
