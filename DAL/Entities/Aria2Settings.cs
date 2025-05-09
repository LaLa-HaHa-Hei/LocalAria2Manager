namespace DAL.Entities
{
    public class Aria2Settings()
    {
        public int Id { get; set; } = 1; // 始终是1
        public string DirectoryPath { get; set; } = string.Empty;// 程序所在文件夹
        public int RpcListenPort { get; set; }// RPC监听端口
    }
}
