namespace BLL.DTOs
{
    public class Aria2SettingsDto()
    {
        public string DirectoryPath { get; set; } = string.Empty;
        public int RpcListenPort { get; set; }
    }
}
