namespace DAL.Entities
{
    public class UiSettings
    {
        public int Id { get; set; } = 1; // 始终是1
        public bool ShowOnStartup { get; set; } // 启动时是否显示UI
    }
}
