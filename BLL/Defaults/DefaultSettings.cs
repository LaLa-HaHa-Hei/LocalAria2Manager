using DAL.Entities;

namespace BLL.Defaults
{
    internal class DefaultSettings
    {
        public static Aria2Settings DefaultAria2Settings => new()
        {
            DirectoryPath = @".\aria2",
            RpcListenPort = 6800
        };

        public static UiSettings DefaultUiSettings => new()
        {
            ShowOnStartup = false,
        };

        public static WindowBounds DefaultWindowBounds => new()
        {
            Width = 800,
            Height = 600,
            Left = 100,
            Top = 100
        };
    }
}
