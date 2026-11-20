using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Contains("--ui-check"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Environment.Exit(UiCheck.Run());
            }

            if (args.Contains("--play-check"))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Environment.Exit(PlayCheck.Run(args));
            }

            if (args.Contains("--sizes"))
            {
                ApiLayout.PrintSizes();
                return;
            }

            NetSdk.CheckLayouts();
            if (args.Contains("--layout"))
            {
                Console.WriteLine("layout-ok");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string error;
            if (!SdkLifetime.Start(out error))
            {
                MessageBox.Show(error, "IRtekNetSDK API Demo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                Application.Run(new MainForm());
            }
            finally
            {
                SdkLifetime.Stop();
            }
        }
    }

    internal static class SdkLifetime
    {
        static readonly NetSdk.LogCallback LogSink = OnLog;
        public static event Action<string> Log;

        public static bool Start(out string error)
        {
            error = null;
            string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, NetSdk.DllName);
            if (!File.Exists(dllPath))
            {
                error = "找不到 IRtekNetSDK.dll。请用 x86 或 x64 重新生成 IRtekNetSDKApiDemo，生成时会从 library 目录复制原生库。";
                return false;
            }

            int code = NetSdk.net_sdk_init();
            if (code != NetSdk.Ok)
            {
                error = "net_sdk_init 失败：" + NetSdk.ErrorText(code);
                return false;
            }

            NetSdk.net_enable_log(1);
            try
            {
                NetSdk.net_set_log_callback(LogSink, IntPtr.Zero);
            }
            catch (EntryPointNotFoundException)
            {
            }

            return true;
        }

        public static void Stop()
        {
            try
            {
                NetSdk.net_sdk_cleanup();
            }
            catch (Exception)
            {
            }
        }

        public static void Write(string message)
        {
            Action<string> handler = Log;
            if (handler != null)
                handler(message);
        }

        static void OnLog(int level, IntPtr message, IntPtr userData)
        {
            string text = NetSdk.PtrToUtf8(message);
            if (string.IsNullOrWhiteSpace(text))
                return;
            string tag;
            switch (level)
            {
                case 0: tag = "DEBUG"; break;
                case 1: tag = "INFO"; break;
                case 2: tag = "WARN"; break;
                case 3: tag = "ERROR"; break;
                default: tag = "LOG"; break;
            }
            Write("[SDK/" + tag + "] " + text.Trim());
        }
    }
}
