using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace IRtekNetSDKApiDemo
{
    internal sealed class AlarmDownloadResult
    {
        public string LocalDir = "";
        public readonly List<string> Files = new List<string>();
        public string DataJsonPath = "";
        public string SnapshotPath = "";
        public string RecordPath = "";
        public string Log = "";
    }

    internal static class AlarmFileClient
    {
        const string RecordRoot = "/userdata/parkia/record";

        public static AlarmDownloadResult Download(string host, int port, string user, string password,
            AlarmSearchItem item, string rootDir)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentException("SFTP 主机为空");

            string folder = LocalFolder(rootDir, item);
            Directory.CreateDirectory(folder);

            var result = new AlarmDownloadResult { LocalDir = folder };
            var log = new StringBuilder();
            using (var client = new SftpClient(host, port, string.IsNullOrEmpty(user) ? "root" : user, password ?? ""))
            {
                client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(20);
                client.OperationTimeout = TimeSpan.FromSeconds(120);
                client.HostKeyReceived += (sender, e) => { e.CanTrust = true; };
                client.Connect();
                try
                {
                    Pull(client, item.RecordPath, folder, "record", result, log);
                    Pull(client, item.SnapshotPath, folder, "snapshot", result, log);
                    Pull(client, item.DataPath, folder, "data", result, log);
                    if (string.IsNullOrEmpty(result.DataJsonPath))
                        result.DataJsonPath = FindLocalJson(folder);
                    if (string.IsNullOrEmpty(result.SnapshotPath))
                        result.SnapshotPath = FindLocalImage(folder);
                    if (string.IsNullOrEmpty(result.RecordPath))
                        result.RecordPath = FindLocalVideo(folder);
                }
                finally
                {
                    if (client.IsConnected)
                        client.Disconnect();
                }
            }

            result.Log = log.ToString().Trim();
            return result;
        }

        static void Pull(SftpClient client, string remote, string folder, string role,
            AlarmDownloadResult result, StringBuilder log)
        {
            if (string.IsNullOrWhiteSpace(remote))
                return;

            string path = ResolveRemote(client, remote);
            if (string.IsNullOrEmpty(path))
            {
                log.AppendLine(role + " 未找到：" + remote);
                return;
            }

            SftpFileAttributes attrs = client.GetAttributes(path);
            if (attrs.IsDirectory)
            {
                foreach (SftpFile file in client.ListDirectory(path))
                {
                    if (file.IsDirectory)
                        continue;
                    SaveFile(client, file.FullName, folder, role, result, log);
                }
                return;
            }

            SaveFile(client, path, folder, role, result, log);
        }

        static void SaveFile(SftpClient client, string remotePath, string folder, string role,
            AlarmDownloadResult result, StringBuilder log)
        {
            string name = Path.GetFileName(remotePath.Replace('/', Path.DirectorySeparatorChar));
            if (string.IsNullOrWhiteSpace(name))
                name = "file.bin";
            string local = UniquePath(Path.Combine(folder, name));
            using (FileStream stream = File.Create(local))
                client.DownloadFile(remotePath, stream);

            result.Files.Add(local);
            log.AppendLine("已下载 " + remotePath + " -> " + local);
            string lower = name.ToLowerInvariant();
            if (lower == "data.json" || (string.IsNullOrEmpty(result.DataJsonPath) && lower.EndsWith(".json")))
                result.DataJsonPath = local;
            if (IsImage(lower) && string.IsNullOrEmpty(result.SnapshotPath))
                result.SnapshotPath = local;
            if ((role == "record" || IsVideo(lower)) && string.IsNullOrEmpty(result.RecordPath))
                result.RecordPath = local;
        }

        static string ResolveRemote(SftpClient client, string remote)
        {
            string path = Normalize(remote);
            if (client.Exists(path))
                return path;
            if (!path.StartsWith("/"))
            {
                string underRoot = RecordRoot.TrimEnd('/') + "/" + path.TrimStart('/');
                if (client.Exists(underRoot))
                    return underRoot;
            }
            if (path.StartsWith("/userdata/"))
            {
                string alt = RecordRoot.TrimEnd('/') + path.Substring("/userdata".Length);
                if (client.Exists(alt))
                    return alt;
            }
            return "";
        }

        static string Normalize(string path)
        {
            return path.Trim().Replace('\\', '/');
        }

        static string UniquePath(string path)
        {
            if (!File.Exists(path))
                return path;
            string dir = Path.GetDirectoryName(path);
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            int i = 2;
            while (true)
            {
                string next = Path.Combine(dir ?? "", name + "_" + i + ext);
                if (!File.Exists(next))
                    return next;
                i++;
            }
        }

        public static string FindLocalJson(string folder)
        {
            string named = Path.Combine(folder, "data.json");
            if (File.Exists(named))
                return named;
            string[] files = Directory.GetFiles(folder, "*.json");
            return files.Length > 0 ? files[0] : "";
        }

        public static string FindLocalImage(string folder)
        {
            string[] patterns = { "*.jpg", "*.jpeg", "*.png", "*.bmp" };
            foreach (string pattern in patterns)
            {
                string[] files = Directory.GetFiles(folder, pattern);
                if (files.Length > 0)
                    return files[0];
            }
            return "";
        }

        public static string FindLocalVideo(string folder)
        {
            string[] patterns = { "*.mp4", "*.avi", "*.mkv", "*.mov", "*.mpg", "*.mpeg", "*.ts", "*.3gp" };
            foreach (string pattern in patterns)
            {
                string[] files = Directory.GetFiles(folder, pattern);
                if (files.Length > 0)
                    return files[0];
            }
            return "";
        }

        static bool IsImage(string name)
        {
            return name.EndsWith(".jpg") || name.EndsWith(".jpeg") || name.EndsWith(".png") || name.EndsWith(".bmp");
        }

        static bool IsVideo(string name)
        {
            return name.EndsWith(".mp4") || name.EndsWith(".avi") || name.EndsWith(".mkv") ||
                   name.EndsWith(".mov") || name.EndsWith(".mpg") || name.EndsWith(".mpeg") ||
                   name.EndsWith(".ts") || name.EndsWith(".3gp");
        }

        public static string LocalFolder(string rootDir, AlarmSearchItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            return Path.Combine(rootDir ?? "", item.AlarmId + "_" + FormatStamp(item.TimestampUs));
        }

        static string FormatStamp(ulong timestampUs)
        {
            DateTime time = AlarmTime.FromUnixUs(timestampUs);
            if (time == DateTime.MinValue)
                return "unknown";
            return time.ToString("yyyyMMdd_HHmmss");
        }
    }
}
