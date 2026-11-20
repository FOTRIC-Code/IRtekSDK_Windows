using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal static class PlayCheck
    {
        public static int Run(string[] args)
        {
            string path = FindVideo(args);
            var log = new StringBuilder();
            log.AppendLine("file=" + (path ?? ""));
            if (string.IsNullOrEmpty(path))
            {
                Write(log, 1);
                return 1;
            }
            log.AppendLine("bytes=" + new FileInfo(path).Length);

            int raw = RawProbe(path, log);
            int one = Probe("one", path, null, log);
            int two = Probe("two", path, path, log);
            int board = BoardProbe(path, log);
            Write(log, raw != 0 || one != 0 || two != 0 || board != 0 ? 1 : 0);
            return raw != 0 || one != 0 || two != 0 || board != 0 ? 1 : 0;
        }

        static int RawProbe(string path, StringBuilder log)
        {
            using (var form = new Form())
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(80, 80);
                form.Size = new Size(480, 360);
                form.Text = "play-check raw";
                form.TopMost = true;
                var host = new System.Windows.Forms.Integration.ElementHost { Bounds = new Rectangle(8, 8, 440, 300) };
                var media = new System.Windows.Controls.MediaElement
                {
                    LoadedBehavior = System.Windows.Controls.MediaState.Play,
                    UnloadedBehavior = System.Windows.Controls.MediaState.Manual,
                    Stretch = System.Windows.Media.Stretch.Uniform,
                    Width = 440,
                    Height = 300
                };
                var notes = new StringBuilder();
                media.MediaOpened += (sender, e) => notes.AppendLine("raw opened video=" + media.NaturalVideoWidth + "x" + media.NaturalVideoHeight);
                media.MediaFailed += (sender, e) => notes.AppendLine("raw failed " + (e.ErrorException == null ? "" : e.ErrorException.Message));
                host.Child = media;
                form.Controls.Add(host);
                form.Show();
                media.Source = new Uri(path);
                var timer = new Timer { Interval = 2500 };
                int lit = 0;
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    try
                    {
                        notes.AppendLine("raw pos=" + media.Position.TotalSeconds.ToString("0.00")
                            + " box=" + media.ActualWidth.ToString("0") + "x" + media.ActualHeight.ToString("0")
                            + " set=" + media.Width.ToString("0") + "x" + media.Height.ToString("0"));
                    }
                    catch (Exception ex)
                    {
                        notes.AppendLine("raw pos-error " + ex.Message);
                    }
                    lit = CountLitPixels(form, host);
                    notes.AppendLine("raw lit=" + lit);
                    form.Close();
                };
                timer.Start();
                Application.Run(form);
                timer.Dispose();
                log.Append(notes.ToString());
                bool ok = notes.ToString().Contains("raw opened") && lit > 200;
                log.AppendLine("raw result=" + (ok ? "pass" : "fail"));
                return ok ? 0 : 1;
            }
        }

        static int BoardProbe(string path, StringBuilder log)
        {
            using (var form = new Form())
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(40, 40);
                form.Size = new Size(1100, 760);
                form.Text = "play-check board";
                form.TopMost = true;
                var board = new AlarmWatchBoard { Dock = DockStyle.Fill };
                form.Controls.Add(board);
                var first = new AlarmSearchItem { AlarmId = 1, ChannelId = 101, AlarmType = 2, Detail = "a" };
                var second = new AlarmSearchItem { AlarmId = 2, ChannelId = 101, AlarmType = 2, Detail = "b" };
                form.Show();
                board.ShowItems(new List<AlarmSearchItem> { first, second });
                Application.DoEvents();
                var result = new AlarmDownloadResult { RecordPath = path };
                board.CardAt(0).Present(result, 2);
                board.CardAt(1).Present(result, 2);
                var notes = new StringBuilder();
                int lit0 = 0;
                int lit1 = 0;
                string state0 = "";
                string state1 = "";
                var timer = new Timer { Interval = 3500 };
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    AlarmVideoPlayer player0 = FindPlayer(board.CardAt(0));
                    AlarmVideoPlayer player1 = FindPlayer(board.CardAt(1));
                    state0 = player0 == null ? "missing" : player0.DebugState();
                    state1 = player1 == null ? "missing" : player1.DebugState();
                    lit0 = player0 == null ? 0 : CountLitPixels(form, player0);
                    lit1 = player1 == null ? 0 : CountLitPixels(form, player1);
                    form.Close();
                };
                timer.Start();
                Application.Run(form);
                timer.Dispose();
                notes.AppendLine("board 0 " + state0);
                notes.AppendLine("board 0 lit=" + lit0);
                notes.AppendLine("board 1 " + state1);
                notes.AppendLine("board 1 lit=" + lit1);
                bool ok = state0.Contains("opened=True") && state1.Contains("opened=True") && lit0 > 200 && lit1 > 200;
                notes.AppendLine("board result=" + (ok ? "pass" : "fail"));
                log.Append(notes.ToString());
                return ok ? 0 : 1;
            }
        }

        static AlarmVideoPlayer FindPlayer(Control root)
        {
            AlarmVideoPlayer player = root as AlarmVideoPlayer;
            if (player != null)
                return player;
            foreach (Control child in root.Controls)
            {
                AlarmVideoPlayer found = FindPlayer(child);
                if (found != null)
                    return found;
            }
            return null;
        }

        static int Probe(string name, string first, string second, StringBuilder log)
        {
            using (var form = new Form())
            {
                form.StartPosition = FormStartPosition.Manual;
                form.Location = new Point(80, 80);
                form.Size = new Size(second == null ? 480 : 900, 360);
                form.Text = "play-check " + name;
                form.TopMost = true;
                form.ShowInTaskbar = false;
                var left = new AlarmVideoPlayer { Bounds = new Rectangle(8, 8, 440, 300) };
                form.Controls.Add(left);
                AlarmVideoPlayer right = null;
                if (second != null)
                {
                    right = new AlarmVideoPlayer { Bounds = new Rectangle(456, 8, 420, 300) };
                    form.Controls.Add(right);
                }
                var notes = new StringBuilder();
                left.StatusChanged += text => notes.AppendLine(name + " L status " + text);
                left.SecondChanged += secondIndex => notes.AppendLine(name + " L second " + secondIndex);
                if (right != null)
                {
                    right.StatusChanged += text => notes.AppendLine(name + " R status " + text);
                    right.SecondChanged += secondIndex => notes.AppendLine(name + " R second " + secondIndex);
                }
                int leftPixels = 0;
                int rightPixels = 0;
                int leftSecond = 0;
                int rightSecond = 0;
                form.Show();
                var timer = new Timer { Interval = 3500 };
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    notes.AppendLine(name + " L " + left.DebugState());
                    leftSecond = left.CurrentSecond;
                    leftPixels = CountLitPixels(form, left);
                    notes.AppendLine(name + " L lit=" + leftPixels);
                    if (right != null)
                    {
                        notes.AppendLine(name + " R " + right.DebugState());
                        rightSecond = right.CurrentSecond;
                        rightPixels = CountLitPixels(form, right);
                        notes.AppendLine(name + " R lit=" + rightPixels);
                    }
                    form.Close();
                };
                bool started = left.PlayFile(first);
                notes.AppendLine(name + " L play=" + started);
                if (right != null)
                    notes.AppendLine(name + " R play=" + right.PlayFile(second));
                timer.Start();
                Application.Run(form);
                timer.Dispose();
                log.Append(notes.ToString());
                bool leftOk = leftSecond >= 1 && leftPixels > 200;
                bool rightOk = right == null || (rightSecond >= 1 && rightPixels > 200);
                log.AppendLine(name + " result=" + (leftOk && rightOk ? "pass" : "fail"));
                return leftOk && rightOk ? 0 : 1;
            }
        }

        static int CountLitPixels(Form form, Control player)
        {
            Rectangle screen = player.RectangleToScreen(player.ClientRectangle);
            if (screen.Width < 4 || screen.Height < 4)
                return 0;
            using (var bitmap = new Bitmap(screen.Width, screen.Height, PixelFormat.Format24bppRgb))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                    graphics.CopyFromScreen(screen.Location, Point.Empty, screen.Size);
                int lit = 0;
                for (int y = 0; y < bitmap.Height; y += 4)
                {
                    for (int x = 0; x < bitmap.Width; x += 4)
                    {
                        Color color = bitmap.GetPixel(x, y);
                        if (color.R > 18 || color.G > 18 || color.B > 18)
                            lit++;
                    }
                }
                return lit;
            }
        }

        static string FindVideo(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--file" && i + 1 < args.Length && File.Exists(args[i + 1]))
                    return args[i + 1];
            }
            string root = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "alarm_files");
            if (!Directory.Exists(root))
                return null;
            string[] files = Directory.GetFiles(root, "record.mp4", SearchOption.AllDirectories);
            string best = null;
            long size = 0;
            foreach (string file in files)
            {
                long length = new FileInfo(file).Length;
                if (length > size)
                {
                    size = length;
                    best = file;
                }
            }
            return best;
        }

        static void Write(StringBuilder log, int code)
        {
            string path = Path.Combine(Path.GetTempPath(), "IRtekNetSDKApiDemo_play_check.txt");
            File.WriteAllText(path, log.ToString(), new UTF8Encoding(true));
            Console.WriteLine(log.ToString());
            Console.WriteLine(path);
            Console.WriteLine("exit " + code);
        }
    }
}
