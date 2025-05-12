﻿using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using BLL.Services;
using DAL.DbContexts;
using DAL.Interfaces;
using DAL.Repositories;
using Hardcodet.Wpf.TaskbarNotification;
using UI.ViewModels;

namespace UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private TaskbarIcon _taskbarIcon = null!;
        private Aria2ProcessService _aria2ProcessService = null!;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //获取欲启动进程名，防止运行两次
            string strProcessName = Process.GetCurrentProcess().ProcessName;
            //检查进程是否已经启动，已经启动则显示报错信息退出程序。
            if (Process.GetProcessesByName(strProcessName).Length > 1)
            {
                MessageBox.Show("程序不能运行2次！", "系统错误", MessageBoxButton.OK, MessageBoxImage.Hand, MessageBoxResult.OK);
                Current.Shutdown();
                return; // 否则会继续执行
            }

            AppDbContext appDbContext = new("data.db");
            _aria2ProcessService = new(new Aria2SettingsRepository(appDbContext));
            IAria2SettingsRepository aria2SettingsRepository = new Aria2SettingsRepository(appDbContext);
            IUiSettingsRepository uiSettingsRepository = new UiSettingsRepository(appDbContext);
            IWindowBoundsRepository windowBoundsRepository = new WindowBoundsRepository(appDbContext);

            _taskbarIcon = (TaskbarIcon)FindResource("TaskbarIcon");
            _taskbarIcon.DataContext = new TaskbarIconViewModel(
                new Aria2SettingsService(aria2SettingsRepository),
                new UiSettingsService(uiSettingsRepository),
                new WindowBoundsService(windowBoundsRepository),
                _aria2ProcessService);
            var contextMenu = (ContextMenu)FindResource("ContextMenu");
            contextMenu.DataContext = _taskbarIcon.DataContext;
            _taskbarIcon.ContextMenu = contextMenu;
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            if (_aria2ProcessService?.IsRunning == true)
            {
                await _aria2ProcessService.DisposeAsync();
            }
        }
    }

}
