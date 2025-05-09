using System.Data.SQLite;
using DAL.Entities;

namespace DAL.DbContexts
{
    public class AppDbContext
    {
        private readonly string _connectionString;

        public AppDbContext(string dbPath)
        {
            _connectionString = $"Data Source={dbPath};Version=3;";
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Aria2Settings (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    DirectoryPath TEXT NOT NULL,
                    RpcListenPort INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS UiSettings (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    ShowOnStartup INTEGER NOT NULL
                );

                CREATE TABLE IF NOT EXISTS WindowBounds (
                    Id INTEGER PRIMARY KEY CHECK (Id = 1),
                    Width REAL NOT NULL,
                    Height REAL NOT NULL,
                    Left REAL NOT NULL,
                    Top REAL NOT NULL
                );
            ";
            command.ExecuteNonQuery();
        }

        // 获取 Aria2 设置
        public Aria2Settings? GetAria2Settings()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, DirectoryPath, RpcListenPort FROM Aria2Settings WHERE Id = 1;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Aria2Settings
                {
                    Id = reader.GetInt32(0),
                    DirectoryPath = reader.GetString(1),
                    RpcListenPort = reader.GetInt32(2)
                };
            }

            return null;
        }

        // 保存 Aria2 设置
        public void SaveAria2Settings(Aria2Settings settings)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT OR REPLACE INTO Aria2Settings (Id, DirectoryPath, RpcListenPort)
                VALUES (@Id, @DirectoryPath, @RpcListenPort);
            ";
            command.Parameters.AddWithValue("@Id", 1);
            command.Parameters.AddWithValue("@DirectoryPath", settings.DirectoryPath);
            command.Parameters.AddWithValue("@RpcListenPort", settings.RpcListenPort);
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        // 获取 UI 设置
        public UiSettings? GetUiSettings()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, ShowOnStartup FROM UiSettings WHERE Id = 1;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new UiSettings
                {
                    Id = reader.GetInt32(0),
                    ShowOnStartup = reader.GetInt32(1) == 1,
                };
            }

            return null;
        }

        // 保存 UI 设置
        public void SaveUiSettings(UiSettings settings)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT OR REPLACE INTO UiSettings (Id, ShowOnStartup)
                VALUES (@Id, @ShowOnStartup);
            ";
            command.Parameters.AddWithValue("@Id", 1);
            command.Parameters.AddWithValue("@ShowOnStartup", settings.ShowOnStartup ? 1 : 0);
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        // 获取窗口大小和位置
        public WindowBounds? GetWindowBounds()
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Width, Height, Left, Top FROM WindowBounds WHERE Id = 1;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new WindowBounds
                {
                    Id = reader.GetInt32(0),
                    Width = reader.GetDouble(1),
                    Height = reader.GetDouble(2),
                    Left = reader.GetDouble(3),
                    Top = reader.GetDouble(4)
                };
            }

            return null;
        }

        // 保存窗口大小和位置
        public void SaveWindowBounds(WindowBounds windowBounds)
        {
            using var connection = new SQLiteConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            using var command = connection.CreateCommand();

            command.CommandText = @"
                INSERT OR REPLACE INTO WindowBounds (Id, Width, Height, Left, Top)
                VALUES (@Id, @Width, @Height, @Left, @Top);
            ";
            command.Parameters.AddWithValue("@Id", 1);
            command.Parameters.AddWithValue("@Width", windowBounds.Width);
            command.Parameters.AddWithValue("@Height", windowBounds.Height);
            command.Parameters.AddWithValue("@Left", windowBounds.Left);
            command.Parameters.AddWithValue("@Top", windowBounds.Top);
            command.ExecuteNonQuery();

            transaction.Commit();
        }
    }
}
