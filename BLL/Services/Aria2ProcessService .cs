using System.Diagnostics;
using System.Text;
using System.Text.Json;
using BLL.Defaults;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services
{
    public class Aria2ProcessService(IAria2SettingsRepository aria2SettingsRepository) : IDisposable, IAsyncDisposable
    {
        private static readonly HttpClient _httpClient = new();
        private readonly IAria2SettingsRepository _aria2SettingsRepository = aria2SettingsRepository;
        private readonly StringBuilder _outputLog = new(); // For capturing output
        private readonly StringBuilder _errorLog = new(); // For capturing error
        private readonly string _aria2AppName = "aria2c.exe";
        private readonly string _aria2ConfigName = "aria2.conf";
        private readonly string _aria2SessionName = "aria2.session";
        private bool _disposed = false;
        private Process? _aria2Process;
        private Aria2Settings? _aria2Settings;

        public string GetOutputLog() => _outputLog.ToString();
        public string GetErrorLog() => _errorLog.ToString();
        public bool IsRunning => _aria2Process != null && !_aria2Process.HasExited;

        public void Start()
        {
            if (IsRunning)
                return;

            _outputLog.Clear();
            _errorLog.Clear();
            // 如果有设置则加载，否则创建默认设置
            _aria2Settings = _aria2SettingsRepository.GetSettings();
            if (_aria2Settings == null)
            {
                _aria2Settings = DefaultSettings.DefaultAria2Settings;
                _aria2SettingsRepository.SaveSettings(_aria2Settings);
            }

            string aria2AppPath = Path.Combine(_aria2Settings.DirectoryPath, _aria2AppName);
            string aria2ConfigPath = Path.Combine(_aria2Settings.DirectoryPath, _aria2ConfigName);
            string aria2SessionPath = Path.Combine(_aria2Settings.DirectoryPath, _aria2SessionName);
            if (!File.Exists(aria2AppPath))
                throw new FileNotFoundException($"{aria2AppPath} not found", aria2AppPath);
            if (!File.Exists(aria2ConfigPath))
                throw new FileNotFoundException($"{aria2ConfigPath} not found", aria2ConfigPath);
            if (!File.Exists(aria2SessionPath))
                File.Create(aria2SessionPath).Dispose();

            _aria2Process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = aria2AppPath,
                    Arguments = $"--conf-path=aria2.conf --rpc-listen-port={_aria2Settings.RpcListenPort}",
                    WorkingDirectory = _aria2Settings.DirectoryPath,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                },
                EnableRaisingEvents = true
            };

            _aria2Process.Start();

            // 启动异步读取输出
            _ = Task.Run(() => ReadOutputAsync(_aria2Process.StandardOutput));
            _ = Task.Run(() => ReadErrorAsync(_aria2Process.StandardError));
        }
        private async Task ReadOutputAsync(StreamReader reader)
        {
            var buffer = new char[1024];
            int charsRead;
            var lineBuilder = new StringBuilder();

            while ((charsRead = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                lock (_outputLog)
                {
                    for (int i = 0; i < charsRead; i++)
                    {
                        char c = buffer[i];

                        if (c == '\r')
                        {
                            // 遇到回车符，覆盖当前行
                            OverwriteLastLine(_outputLog, lineBuilder.ToString());
                            lineBuilder.Clear();
                        }
                        else if (c == '\n')
                        {
                            // 遇到换行符，正常换行
                            _outputLog.AppendLine(lineBuilder.ToString());
                            lineBuilder.Clear();
                        }
                        else
                        {
                            lineBuilder.Append(c);
                        }
                    }
                }
            }
        }

        private async Task ReadErrorAsync(StreamReader reader)
        {
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                lock (_errorLog)
                {
                    _errorLog.AppendLine(line);
                }
            }
        }

        /// <summary>
        /// 覆盖 StringBuilder 中最后一行
        /// </summary>
        private static void OverwriteLastLine(StringBuilder sb, string newLine)
        {
            int lastNewLineIndex = sb.ToString().LastIndexOf('\n');

            if (lastNewLineIndex >= 0)
            {
                sb.Remove(lastNewLineIndex + 1, sb.Length - (lastNewLineIndex + 1));
            }
            else
            {
                sb.Clear();
            }

            sb.Append(newLine);
        }

        public async Task StopAsync()
        {
            if (IsRunning)
            {
                try
                {
                    // 如果正在运行，那么在Start()方法时一定给_aria2Settings赋值了
                    // 发送 HTTP 请求来关闭 aria2
                    await SendShutdownRequestAsync(Guid.NewGuid().ToString(), _aria2Settings!.RpcListenPort);

                    // 等待进程退出
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)); // aria2一般需要3秒去退出
                    try
                    {
                        await _aria2Process.WaitForExitAsync(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        Debug.WriteLine("Aria2 did not exit gracefully after 5 seconds. Killing process.");
                        _aria2Process.Kill();
                        using var killCts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                        await _aria2Process.WaitForExitAsync(killCts.Token);
                    }
                }
                catch (Exception)
                {
                    if (!_aria2Process?.HasExited == true) // Check if it hasn't exited due to the exception or other reasons
                    {
                        try
                        {
                            _aria2Process.Kill();
                            using var killCts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                            await _aria2Process.WaitForExitAsync(killCts.Token);
                        }
                        catch (Exception killEx)
                        {
                            Debug.WriteLine($"Failed to kill aria2 process: {killEx}");
                        }
                    }
                }
            }
        }

        private static async Task SendShutdownRequestAsync(string id, int port)
        {
            var requestData = new
            {
                jsonrpc = "2.0",
                method = "aria2.shutdown",
                id
            };

            var jsonContent = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"http://localhost:{port}/jsonrpc", content);
            response.EnsureSuccessStatusCode();

            //var responseContent = await response.Content.ReadAsStringAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync() // 实现 IAsyncDisposable
        {
            if (_disposed)
                return;

            await StopAsync();

            _aria2Process?.Dispose();
            _aria2Process = null;


            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // 释放非托管资源
            if (IsRunning)
            {
                try
                {
                    _aria2Process!.Kill();
                    _aria2Process!.WaitForExit(1000); // 短暂等待
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Synchronous Dispose: Error during Aria2 stop/kill: {ex.Message}");
                }
            }

            // 释放托管资源
            if (disposing)
            {
                _aria2Process?.Dispose();
            }

            _disposed = true;
        }

        ~Aria2ProcessService()
        {
            Dispose(false);
        }
    }
}

