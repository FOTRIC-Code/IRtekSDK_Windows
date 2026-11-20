using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRtekNetSDKApiDemo
{
    internal enum ApiParamKind
    {
        Number,
        Enum
    }

    internal sealed class EnumChoice
    {
        public EnumChoice(int value, string text)
        {
            Value = value;
            Text = text;
        }

        public int Value;
        public string Text;

        public override string ToString()
        {
            return Value.ToString() + "  " + Text;
        }
    }

    internal sealed class ApiParam
    {
        public string Key;
        public string Label;
        public ApiParamKind Kind;
        public int DefaultValue;
        public EnumChoice[] Options;
    }

    internal sealed class ApiItem
    {
        public string Group;
        public string Name;
        public string Summary;
        public bool Dangerous;
        public string Method;
        public string Path;
        public string RequestExample;
        public string ResponseExample;
        public ApiParam[] Params;
        public Func<ApiCallContext, string> Run;
    }

    internal sealed class ApiCallContext
    {
        public IntPtr Handle;
        public uint StreamId;
        public uint ChannelId;
        public uint NetworkId;
        public uint StorageId;
        public uint UserId;
        public int AlarmType;
        public int CaptureMode;
        public uint PageNo;
        public uint PageSize;
        public string FilePath;
        public Form Owner;
    }

    internal sealed partial class ApiTestForm : Form
    {
        readonly DeviceSession _session;
        readonly List<ApiItem> _items = new List<ApiItem>();
        readonly Dictionary<string, Control> _paramControls = new Dictionary<string, Control>();
        readonly Dictionary<string, int> _paramValues = new Dictionary<string, int>();
        bool _working;

        public event Action DeviceInvalidated;

        public ApiTestForm()
        {
            InitializeComponent();
        }

        public ApiTestForm(DeviceSession session) : this()
        {
            _session = session;
            if (session != null)
                Text = "接口测试 - " + session.Ip;
            BuildCatalog();
            FillTree();

            _tree.AfterSelect += (sender, e) => ShowSelected();
            _tree.NodeMouseDoubleClick += async (sender, e) => await InvokeSelectedAsync();
            _invoke.Click += async (sender, e) => await InvokeSelectedAsync();
            _runGroup.Click += async (sender, e) => await RunManyAsync(SelectedGroupItems());
            _runAll.Click += async (sender, e) => await RunManyAsync(_items);
            ShowSelected();
        }

        void FillTree()
        {
            _tree.BeginUpdate();
            _tree.Nodes.Clear();
            TreeNode group = null;
            foreach (ApiItem item in _items)
            {
                if (group == null || group.Text != item.Group)
                {
                    group = _tree.Nodes.Add(item.Group);
                    group.NodeFont = new Font(Font, FontStyle.Bold);
                }
                TreeNode node = group.Nodes.Add(item.Name);
                node.Tag = item;
            }
            _tree.ExpandAll();
            if (_tree.Nodes.Count > 0 && _tree.Nodes[0].Nodes.Count > 0)
                _tree.SelectedNode = _tree.Nodes[0].Nodes[0];
            _tree.EndUpdate();
        }

        ApiItem SelectedItem()
        {
            return _tree.SelectedNode != null ? _tree.SelectedNode.Tag as ApiItem : null;
        }

        List<ApiItem> SelectedGroupItems()
        {
            var list = new List<ApiItem>();
            TreeNode node = _tree.SelectedNode;
            if (node == null)
                return list;
            TreeNode group = node.Tag is ApiItem ? node.Parent : node;
            if (group == null)
                return list;
            foreach (TreeNode child in group.Nodes)
            {
                ApiItem item = child.Tag as ApiItem;
                if (item != null)
                    list.Add(item);
            }
            return list;
        }

        void ShowSelected()
        {
            ApiItem item = SelectedItem();
            if (item == null)
            {
                _title.Text = "选择左侧接口后调用";
                _summary.Text = "双击或点“调用”。需要参数的获取接口可在上方填写或选择。";
                _send.Text = "";
                _receive.Text = "";
                _endpoint.Text = "";
                BindParams(null);
                return;
            }
            _title.Text = item.Name + (item.Dangerous ? "  [危险]" : "");
            _summary.Text = item.Summary;
            _send.Text = string.IsNullOrEmpty(item.RequestExample) ? "{}" : item.RequestExample;
            _receive.Text = string.IsNullOrEmpty(item.ResponseExample) ? "{}" : item.ResponseExample;
            BindParams(item);
            _endpoint.Text = FormatEndpoint(item, Context());
        }

        ApiCallContext Context()
        {
            return new ApiCallContext
            {
                Handle = _session != null ? _session.Handle : IntPtr.Zero,
                StreamId = (uint)ParamValue("stream_id", (int)ChannelIds.Default),
                ChannelId = (uint)ParamValue("channel_id", (int)ChannelIds.Default),
                NetworkId = (uint)ParamValue("network_id", 0),
                StorageId = (uint)ParamValue("storage_id", 0),
                UserId = (uint)ParamValue("user_id", 0),
                AlarmType = ParamValue("alarm_type", 1),
                CaptureMode = ParamValue("capture_mode", 1),
                PageNo = (uint)ParamValue("page_no", 0),
                PageSize = (uint)ParamValue("page_size", 20),
                FilePath = "",
                Owner = this
            };
        }

        void BindParams(ApiItem item)
        {
            _paramBar.SuspendLayout();
            _paramBar.Controls.Clear();
            _paramControls.Clear();
            ApiParam[] args = item == null ? null : item.Params;
            if (args == null || args.Length == 0)
            {
                _paramBar.Visible = false;
                _paramBar.ResumeLayout();
                return;
            }

            var caption = new Label
            {
                Text = "调用参数",
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Margin = new Padding(0, 8, 12, 0)
            };
            _paramBar.Controls.Add(caption);

            foreach (ApiParam param in args)
            {
                var label = new Label
                {
                    Text = param.Label,
                    AutoSize = true,
                    Margin = new Padding(0, 8, 6, 0)
                };
                _paramBar.Controls.Add(label);

                int current = ParamValue(param.Key, param.DefaultValue);
                Control editor;
                if (param.Kind == ApiParamKind.Enum)
                {
                    var box = new ComboBox
                    {
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Width = 260,
                        DropDownWidth = 320,
                        Margin = new Padding(0, 4, 16, 4)
                    };
                    if (param.Options != null)
                    {
                        foreach (EnumChoice choice in param.Options)
                            box.Items.Add(choice);
                    }
                    SelectChoice(box, current);
                    string key = param.Key;
                    box.SelectedIndexChanged += (sender, e) =>
                    {
                        EnumChoice choice = box.SelectedItem as EnumChoice;
                        if (choice != null)
                            Remember(key, choice.Value);
                        RefreshEndpoint();
                    };
                    editor = box;
                }
                else
                {
                    if (current < 0)
                        current = 0;
                    if (current > 65535)
                        current = 65535;
                    var box = new NumericUpDown
                    {
                        Minimum = 0,
                        Maximum = 65535,
                        DecimalPlaces = 0,
                        Width = 90,
                        Margin = new Padding(0, 4, 16, 4),
                        Value = current
                    };
                    string key = param.Key;
                    box.ValueChanged += (sender, e) =>
                    {
                        Remember(key, (int)box.Value);
                        RefreshEndpoint();
                    };
                    editor = box;
                }
                _paramControls[param.Key] = editor;
                Remember(param.Key, current);
                _paramBar.Controls.Add(editor);
            }

            _paramBar.Visible = true;
            _paramBar.ResumeLayout();
        }

        void RefreshEndpoint()
        {
            ApiItem item = SelectedItem();
            if (item != null)
                _endpoint.Text = FormatEndpoint(item, Context());
        }

        void Remember(string key, int value)
        {
            _paramValues[key] = value;
        }

        int ParamValue(string key, int fallback)
        {
            int value;
            if (_paramValues.TryGetValue(key, out value))
                return value;
            return fallback;
        }

        static void SelectChoice(ComboBox box, int value)
        {
            for (int i = 0; i < box.Items.Count; i++)
            {
                EnumChoice choice = box.Items[i] as EnumChoice;
                if (choice != null && choice.Value == value)
                {
                    box.SelectedIndex = i;
                    return;
                }
            }
            if (box.Items.Count > 0)
                box.SelectedIndex = 0;
        }

        async Task InvokeSelectedAsync()
        {
            ApiItem item = SelectedItem();
            if (item == null)
                return;
            await RunManyAsync(new List<ApiItem> { item });
        }

        async Task RunManyAsync(List<ApiItem> items)
        {
            if (_working || items == null || items.Count == 0)
                return;
            if (_session == null || _session.Handle == IntPtr.Zero)
            {
                MessageBox.Show(this, "当前没有已连接的设备。", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var todo = new List<ApiItem>();
            foreach (ApiItem item in items)
            {
                if (item.Dangerous && !_includeDanger.Checked && items.Count > 1)
                    continue;
                if (item.Dangerous && !ConfirmDanger(item))
                    continue;
                todo.Add(item);
            }
            if (todo.Count == 0)
                return;

            _working = true;
            SetBusy(true);
            ApiCallContext ctx = Context();
            var parts = new List<string>();
            try
            {
                foreach (ApiItem item in todo)
                {
                    if (item.Name == "net_device_upgrade" && string.IsNullOrWhiteSpace(ctx.FilePath))
                    {
                        using (var dialog = new OpenFileDialog())
                        {
                            dialog.Filter = "固件 (*.bin)|*.bin|所有文件 (*.*)|*.*";
                            if (dialog.ShowDialog(this) != DialogResult.OK)
                                continue;
                            ctx.FilePath = dialog.FileName;
                        }
                    }
                    string text = await Task.Run(() => RunSafe(item, ctx));
                    parts.Add("{\r\n  \"api\": " + Quote(item.Name) + ",\r\n  \"result\": " + IndentJson(text, 1).Trim() + "\r\n}");
                    if (todo.Count == 1)
                        ShowCallResult(item, ctx, text);
                    else
                        ShowCallResult(item, ctx, "[\r\n" + string.Join(",\r\n", parts.ToArray()) + "\r\n]");
                    SdkLifetime.Write(item.Name + " 已调用");
                    if (item.Name.IndexOf("restart", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        item.Name.IndexOf("reset", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        BeginInvoke(new Action(() =>
                        {
                            Action handler = DeviceInvalidated;
                            if (handler != null)
                                handler();
                        }));
                    }
                }
            }
            finally
            {
                _working = false;
                SetBusy(false);
            }
        }

        bool ConfirmDanger(ApiItem item)
        {
            return MessageBox.Show(
                this,
                item.Name + " 会改设备状态，确认调用？\r\n" + item.Summary,
                Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }

        static string RunSafe(ApiItem item, ApiCallContext ctx)
        {
            try
            {
                return item.Run(ctx);
            }
            catch (Exception ex)
            {
                return "异常：" + ex.Message;
            }
        }

        void SetBusy(bool busy)
        {
            _tree.Enabled = !busy;
            _invoke.Enabled = !busy;
            _runGroup.Enabled = !busy;
            _runAll.Enabled = !busy;
            _paramBar.Enabled = !busy;
            UseWaitCursor = busy;
        }

        void ShowCallResult(ApiItem item, ApiCallContext ctx, string received)
        {
            _endpoint.Text = FormatEndpoint(item, ctx);
            _receive.Text = string.IsNullOrEmpty(received) ? "{}" : received;
        }

        string FormatEndpoint(ApiItem item, ApiCallContext ctx)
        {
            string path = ResolvePath(item.Path, ctx);
            string url;
            if (string.IsNullOrEmpty(path))
                url = "";
            else if (path.StartsWith("/"))
            {
                string ip = _session != null ? _session.Ip : "";
                int port = _session != null ? _session.HttpPort : 0;
                url = "http://" + ip + ":" + port + path;
            }
            else
                url = path;
            return "{\r\n  \"sdk\": " + Quote(item.Name) + ",\r\n  \"method\": " + Quote(item.Method) + ",\r\n  \"url\": " + Quote(url) + "\r\n}";
        }

        static string ResolvePath(string path, ApiCallContext ctx)
        {
            if (string.IsNullOrEmpty(path))
                return "";
            return path
                .Replace("{type}", ctx.AlarmType.ToString())
                .Replace("{stream_id}", ctx.StreamId.ToString())
                .Replace("{channel_id}", ctx.ChannelId.ToString())
                .Replace("{id}", ctx.NetworkId.ToString())
                .Replace("{storage_id}", ctx.StorageId.ToString())
                .Replace("{user_id}", ctx.UserId.ToString())
                .Replace("{mode}", ctx.CaptureMode.ToString());
        }

        void BuildCatalog()
        {
            Add("SDK", "net_get_sdk_version", "读取 SDK 版本号", false, ctx =>
            {
                NetSdkVersion ver;
                int code = NetSdk.net_get_sdk_version(out ver);
                return Result(code, Dump(ver));
            });

            Add("设备管理", "net_get_device_info", "读取设备名称、型号、固件等", false, ctx =>
            {
                NetDeviceInfo info = NetDeviceInfo.CreateBlank();
                int code = NetSdk.net_get_device_info(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_device_info", "把当前设备信息原样写回", false, ctx =>
            {
                NetDeviceInfo info = NetDeviceInfo.CreateBlank();
                int get = NetSdk.net_get_device_info(ctx.Handle, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_device_info(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("设备管理", "net_get_time", "读取设备时间", false, ctx =>
            {
                NetTimeInfo info = NetTimeInfo.CreateBlank();
                int code = NetSdk.net_get_time(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_time", "把当前时间配置原样写回", false, ctx => RoundTrip(
                () => { var info = NetTimeInfo.CreateBlank(); int c = NetSdk.net_get_time(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetTimeInfo)info; return NetSdk.net_set_time(ctx.Handle, ref value); }));
            Add("设备管理", "net_get_ntp_server", "读取 NTP 服务器", false, ctx =>
            {
                NetNtpInfo info = NetNtpInfo.CreateBlank();
                int code = NetSdk.net_get_ntp_server(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_ntp_server", "把当前 NTP 配置原样写回", false, ctx => RoundTrip(
                () => { var info = NetNtpInfo.CreateBlank(); int c = NetSdk.net_get_ntp_server(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetNtpInfo)info; return NetSdk.net_set_ntp_server(ctx.Handle, ref value); }));
            Add("设备管理", "net_get_port", "读取 HTTP/RTSP/RTP 端口", false, ctx =>
            {
                NetPortInfo info;
                int code = NetSdk.net_get_port(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_port", "把当前端口配置原样写回", true, ctx =>
            {
                NetPortInfo info;
                int get = NetSdk.net_get_port(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_port(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("设备管理", "net_get_networks", "读取网卡列表", false, ctx =>
            {
                NetNetworkInfo[] list = CreateArray(8, NetNetworkInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_networks(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("设备管理", "net_get_network", "按网卡 ID 读取", false, NumberParam("network_id", "网卡 ID"), ctx =>
            {
                NetNetworkInfo info = NetNetworkInfo.CreateBlank();
                int code = NetSdk.net_get_network(ctx.Handle, ctx.NetworkId, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_network", "把指定网卡当前值写回", true, NumberParam("network_id", "网卡 ID"), ctx =>
            {
                NetNetworkInfo info = NetNetworkInfo.CreateBlank();
                int get = NetSdk.net_get_network(ctx.Handle, ctx.NetworkId, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_network(ctx.Handle, ctx.NetworkId, ref info);
                return Result(set, Dump(info));
            });
            Add("设备管理", "net_get_ftp", "读取 FTP 配置", false, ctx =>
            {
                NetFtpInfo info = NetFtpInfo.CreateBlank();
                int code = NetSdk.net_get_ftp(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_ftp", "把当前 FTP 配置原样写回", false, ctx => RoundTrip(
                () => { var info = NetFtpInfo.CreateBlank(); int c = NetSdk.net_get_ftp(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetFtpInfo)info; return NetSdk.net_set_ftp(ctx.Handle, ref value); }));
            Add("设备管理", "net_test_ftp", "用当前 FTP 配置做连通测试", false, ctx => RoundTrip(
                () => { var info = NetFtpInfo.CreateBlank(); int c = NetSdk.net_get_ftp(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetFtpInfo)info; return NetSdk.net_test_ftp(ctx.Handle, ref value); }));
            Add("设备管理", "net_get_webhook", "读取最近写入的 Webhook", false, ctx =>
            {
                NetWebhookInfo info = NetWebhookInfo.CreateBlank();
                int code = NetSdk.net_get_webhook(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("设备管理", "net_set_webhook", "把当前 Webhook 原样写回", false, ctx => RoundTrip(
                () => { var info = NetWebhookInfo.CreateBlank(); int c = NetSdk.net_get_webhook(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetWebhookInfo)info; return NetSdk.net_set_webhook(ctx.Handle, ref value); }));
            Add("设备管理", "net_get_webhooks", "读取 Webhook 列表", false, ctx =>
            {
                NetWebhookInfo[] list = CreateArray(NetSdk.MaxWebhooks, NetWebhookInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_webhooks(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("设备管理", "net_set_webhooks", "把当前 Webhook 列表原样写回", false, ctx =>
            {
                NetWebhookInfo[] list = CreateArray(NetSdk.MaxWebhooks, NetWebhookInfo.CreateBlank);
                ushort count;
                int get = NetSdk.net_get_webhooks(ctx.Handle, list, (ushort)list.Length, out count);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_webhooks(ctx.Handle, list, count);
                return Result(set, DumpArray(list, count));
            });
            Add("设备管理", "net_device_upgrade", "调用时选择固件包并上传", true, ctx =>
            {
                if (string.IsNullOrWhiteSpace(ctx.FilePath) || !File.Exists(ctx.FilePath))
                    return "{\r\n  \"error\": \"未选择固件文件\"\r\n}";
                NetUpgradeInfo info = NetUpgradeInfo.FromFile(ctx.FilePath);
                int code = NetSdk.net_device_upgrade(ctx.Handle, ref info);
                return Result(code, JsonKv("path", ctx.FilePath));
            });
            Add("设备管理", "net_device_restart", "重启设备，调用后连接会失效", true, ctx => Result(NetSdk.net_device_restart(ctx.Handle), ""));
            Add("设备管理", "net_device_reset", "恢复出厂，调用后连接会失效", true, ctx => Result(NetSdk.net_device_reset(ctx.Handle), ""));

            Add("用户", "net_get_users", "读取用户列表", false, ctx =>
            {
                NetUserInfo[] list = CreateArray(16, NetUserInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_users(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("用户", "net_get_user", "按用户 ID 读取", false, NumberParam("user_id", "用户 ID"), ctx =>
            {
                NetUserInfo info = NetUserInfo.CreateBlank();
                int code = NetSdk.net_get_user(ctx.Handle, ctx.UserId, ref info);
                return Result(code, Dump(info));
            });

            Add("视频流", "net_get_streams", "读取视频流列表", false, ctx =>
            {
                NetStreamInfo[] list = CreateArray(8, NetStreamInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_streams(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("视频流", "net_get_stream", "按流 ID 读取参数", false, NumberParam("stream_id", "流 ID", (int)ChannelIds.Default), ctx =>
            {
                NetStreamInfo info = NetStreamInfo.CreateBlank();
                int code = NetSdk.net_get_stream(ctx.Handle, ctx.StreamId, ref info);
                return Result(code, Dump(info));
            });
            Add("视频流", "net_get_stream_status", "按流 ID 读取连接数", false, NumberParam("stream_id", "流 ID", (int)ChannelIds.Default), ctx =>
            {
                NetStreamStatus status;
                int code = NetSdk.net_get_stream_status(ctx.Handle, ctx.StreamId, out status);
                return Result(code, Dump(status));
            });

            Add("显示", "net_get_channels", "读取通道列表", false, ctx =>
            {
                NetChannelInfo[] list = CreateArray(8, NetChannelInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_channels(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("显示", "net_get_channel", "按通道 ID 读取", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                NetChannelInfo info = NetChannelInfo.CreateBlank();
                int code = NetSdk.net_get_channel(ctx.Handle, ctx.ChannelId, ref info);
                return Result(code, Dump(info));
            });
            Add("显示", "net_set_channel", "把当前通道配置原样写回", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx => RoundTrip(
                () => { var info = NetChannelInfo.CreateBlank(); int c = NetSdk.net_get_channel(ctx.Handle, ctx.ChannelId, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetChannelInfo)info; return NetSdk.net_set_channel(ctx.Handle, ctx.ChannelId, ref value); }));
            Add("显示", "net_get_overlay", "读取字符叠加", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                NetOverlayInfo info = NetOverlayInfo.CreateBlank();
                int code = NetSdk.net_get_overlay(ctx.Handle, ctx.ChannelId, ref info);
                return Result(code, Dump(info));
            });
            Add("显示", "net_set_overlay", "把当前字符叠加原样写回", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx => RoundTrip(
                () => { var info = NetOverlayInfo.CreateBlank(); int c = NetSdk.net_get_overlay(ctx.Handle, ctx.ChannelId, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetOverlayInfo)info; return NetSdk.net_set_overlay(ctx.Handle, ctx.ChannelId, ref value); }));
            Add("显示", "net_get_image_params", "读取图像参数", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                NetImageParams info;
                int code = NetSdk.net_get_image_params(ctx.Handle, ctx.ChannelId, out info);
                return Result(code, Dump(info));
            });
            Add("显示", "net_set_image_params", "把当前图像参数原样写回", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                NetImageParams info;
                int get = NetSdk.net_get_image_params(ctx.Handle, ctx.ChannelId, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_image_params(ctx.Handle, ctx.ChannelId, ref info);
                return Result(set, Dump(info));
            });
            Add("显示", "net_get_image_flip", "读取翻转模式", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                int mode;
                int code = NetSdk.net_get_image_flip(ctx.Handle, ctx.ChannelId, out mode);
                return Result(code, JsonKv("mode", mode));
            });
            Add("显示", "net_set_image_flip", "把当前翻转模式原样写回", false, NumberParam("channel_id", "通道 ID", (int)ChannelIds.Default), ctx =>
            {
                int mode;
                int get = NetSdk.net_get_image_flip(ctx.Handle, ctx.ChannelId, out mode);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_image_flip(ctx.Handle, ctx.ChannelId, ref mode);
                return Result(set, JsonKv("mode", mode));
            });

            Add("热像", "net_get_thermal_lens_info", "读取镜头和量程", false, ctx =>
            {
                NetThermalLensInfo info = NetThermalLensInfo.CreateBlank();
                int code = NetSdk.net_get_thermal_lens_info(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_thermal_calibrate", "执行一次快门校正", false, ctx => Result(NetSdk.net_thermal_calibrate(ctx.Handle), ""));
            Add("热像", "net_get_thermal_shutter_mode", "读取快门模式", false, ctx =>
            {
                int mode;
                int code = NetSdk.net_get_thermal_shutter_mode(ctx.Handle, out mode);
                return Result(code, JsonKv("mode", mode));
            });
            Add("热像", "net_set_thermal_shutter_mode", "把当前快门模式原样写回", false, ctx =>
            {
                int mode;
                int get = NetSdk.net_get_thermal_shutter_mode(ctx.Handle, out mode);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_shutter_mode(ctx.Handle, ref mode);
                return Result(set, JsonKv("mode", mode));
            });
            Add("热像", "net_get_thermal_shutter_time", "读取自动校正间隔", false, ctx =>
            {
                ushort minutes;
                int code = NetSdk.net_get_thermal_shutter_time(ctx.Handle, out minutes);
                return Result(code, JsonKv("minutes", minutes));
            });
            Add("热像", "net_set_thermal_shutter_time", "把当前自动校正间隔原样写回", false, ctx =>
            {
                ushort minutes;
                int get = NetSdk.net_get_thermal_shutter_time(ctx.Handle, out minutes);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_shutter_time(ctx.Handle, ref minutes);
                return Result(set, JsonKv("minutes", minutes));
            });
            Add("热像", "net_get_thermal_resolution", "读取热像分辨率", false, ctx =>
            {
                NetSize size;
                int code = NetSdk.net_get_thermal_resolution(ctx.Handle, out size);
                return Result(code, Dump(size));
            });
            Add("热像", "net_get_thermal_range", "读取量程", false, ctx =>
            {
                ushort rangeId;
                int code = NetSdk.net_get_thermal_range(ctx.Handle, out rangeId);
                return Result(code, JsonKv("range_id", rangeId));
            });
            Add("热像", "net_set_thermal_range", "把当前量程原样写回", false, ctx =>
            {
                ushort rangeId;
                int get = NetSdk.net_get_thermal_range(ctx.Handle, out rangeId);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_range(ctx.Handle, ref rangeId);
                return Result(set, JsonKv("range_id", rangeId));
            });
            Add("热像", "net_get_thermal_lut", "读取 LUT 表", false, ctx =>
            {
                NetLutTable lut = NetLutTable.CreateBlank();
                int code = NetSdk.net_get_thermal_lut(ctx.Handle, ref lut);
                return Result(code, Dump(lut));
            });
            Add("热像", "net_get_thermal_params", "读取测温参数", false, ctx =>
            {
                NetThermalParams info;
                int code = NetSdk.net_get_thermal_params(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_params", "把当前测温参数原样写回", false, ctx =>
            {
                NetThermalParams info;
                int get = NetSdk.net_get_thermal_params(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_params(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_color_dist", "读取色彩分布", false, ctx =>
            {
                int type;
                int code = NetSdk.net_get_thermal_color_dist(ctx.Handle, out type);
                return Result(code, JsonKv("type", type));
            });
            Add("热像", "net_set_thermal_color_dist", "把当前色彩分布原样写回", false, ctx =>
            {
                int type;
                int get = NetSdk.net_get_thermal_color_dist(ctx.Handle, out type);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_color_dist(ctx.Handle, ref type);
                return Result(set, JsonKv("type", type));
            });
            Add("热像", "net_get_thermal_gain", "读取温宽", false, ctx =>
            {
                NetGain info = new NetGain();
                int code = NetSdk.net_get_thermal_gain(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_gain", "把当前温宽原样写回", false, ctx =>
            {
                NetGain info = new NetGain();
                int get = NetSdk.net_get_thermal_gain(ctx.Handle, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_gain(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_preset_palettes", "读取预置调色板列表", false, ctx =>
            {
                NetPresetPalette[] list = CreateArray(32, NetPresetPalette.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_thermal_preset_palettes(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("热像", "net_get_thermal_palette", "读取当前调色板", false, ctx =>
            {
                NetPalette info = NetPalette.CreateBlank();
                int code = NetSdk.net_get_thermal_palette(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_palette", "把当前调色板原样写回", false, ctx => RoundTrip(
                () => { var info = NetPalette.CreateBlank(); int c = NetSdk.net_get_thermal_palette(ctx.Handle, ref info); return Tuple.Create(c, (object)info); },
                info => { var value = (NetPalette)info; return NetSdk.net_set_thermal_palette(ctx.Handle, ref value); }));
            Add("热像", "net_get_thermal_overlay", "读取热像叠加", false, ctx =>
            {
                NetThermalOverlayInfo info = new NetThermalOverlayInfo();
                int code = NetSdk.net_get_thermal_overlay(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_overlay", "把当前热像叠加原样写回", false, ctx =>
            {
                NetThermalOverlayInfo info = new NetThermalOverlayInfo();
                int get = NetSdk.net_get_thermal_overlay(ctx.Handle, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_overlay(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_unit", "读取温度和距离单位", false, ctx =>
            {
                NetUnitInfo info;
                int code = NetSdk.net_get_thermal_unit(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_unit", "把当前单位原样写回", false, ctx =>
            {
                NetUnitInfo info;
                int get = NetSdk.net_get_thermal_unit(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_unit(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_capture", "抓拍热图到临时目录", false, ThermalCaptureParam(), ctx =>
            {
                string path = Path.Combine(Path.GetTempPath(), "parkia_thermal.jpg");
                NetCaptureFile file = NetCaptureFile.FromPath(path);
                int code = NetSdk.net_get_thermal_capture(ctx.Handle, ctx.CaptureMode, ref file);
                return Result(code, JsonKv("local_file", path));
            });
            Add("热像", "net_get_thermal_markers", "读取测温对象", false, ctx =>
            {
                NetMarkerInfo[] list = CreateArray(NetSdk.MaxMarkers, NetMarkerInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_thermal_markers(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("热像", "net_set_thermal_markers", "把当前测温对象原样写回", false, ctx =>
            {
                NetMarkerInfo[] list = CreateArray(NetSdk.MaxMarkers, NetMarkerInfo.CreateBlank);
                ushort count;
                int get = NetSdk.net_get_thermal_markers(ctx.Handle, list, (ushort)list.Length, out count);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_markers(ctx.Handle, list, count);
                return Result(set, DumpArray(list, count));
            });
            Add("热像", "net_get_thermal_temp_alarms", "读取测温报警", false, ctx =>
            {
                NetThermalTempAlarm[] list = CreateArray(NetSdk.MaxMarkers, NetThermalTempAlarm.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_thermal_temp_alarms(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("热像", "net_set_thermal_temp_alarms", "把当前测温报警原样写回", false, ctx =>
            {
                NetThermalTempAlarm[] list = CreateArray(NetSdk.MaxMarkers, NetThermalTempAlarm.CreateBlank);
                ushort count;
                int get = NetSdk.net_get_thermal_temp_alarms(ctx.Handle, list, (ushort)list.Length, out count);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_temp_alarms(ctx.Handle, list, count);
                return Result(set, DumpArray(list, count));
            });
            Add("热像", "net_get_thermal_temp_alarm_interval", "读取测温报警间隔", false, ctx =>
            {
                NetThermalTempAlarmInterval info;
                int code = NetSdk.net_get_thermal_temp_alarm_interval(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_temp_alarm_interval", "把当前测温报警间隔原样写回", false, ctx =>
            {
                NetThermalTempAlarmInterval info;
                int get = NetSdk.net_get_thermal_temp_alarm_interval(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_temp_alarm_interval(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_intrusion_alarm", "读取入侵报警", false, ctx =>
            {
                NetIntrusionAlarm info;
                int code = NetSdk.net_get_thermal_intrusion_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_intrusion_alarm", "把当前入侵报警原样写回", false, ctx =>
            {
                NetIntrusionAlarm info;
                int get = NetSdk.net_get_thermal_intrusion_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_intrusion_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_high_temp_alarm", "读取高温报警", false, ctx =>
            {
                NetHighTempAlarm info;
                int code = NetSdk.net_get_thermal_high_temp_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_high_temp_alarm", "把当前高温报警原样写回", false, ctx =>
            {
                NetHighTempAlarm info;
                int get = NetSdk.net_get_thermal_high_temp_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_high_temp_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("热像", "net_get_thermal_temp_detect_alarm", "读取温度监测报警", false, ctx =>
            {
                NetTempDetectAlarm info;
                int code = NetSdk.net_get_thermal_temp_detect_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("热像", "net_set_thermal_temp_detect_alarm", "把当前温度监测报警原样写回", false, ctx =>
            {
                NetTempDetectAlarm info;
                int get = NetSdk.net_get_thermal_temp_detect_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_thermal_temp_detect_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });

            Add("报警", "net_get_alarm_trigger", "读取报警联动", false, AlarmTypeParam(), ctx =>
            {
                int type = ctx.AlarmType;
                NetAlarmTrigger info = new NetAlarmTrigger();
                int code = NetSdk.net_get_alarm_trigger(ctx.Handle, ref type, ref info);
                return Result(code, Dump(info));
            });
            Add("报警", "net_set_alarm_trigger", "把当前报警联动原样写回", false, AlarmTypeParam(), ctx =>
            {
                int type = ctx.AlarmType;
                NetAlarmTrigger info = new NetAlarmTrigger();
                int get = NetSdk.net_get_alarm_trigger(ctx.Handle, ref type, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "");
                type = ctx.AlarmType;
                int set = NetSdk.net_set_alarm_trigger(ctx.Handle, ref type, ref info);
                return Result(set, Dump(info));
            });
            Add("报警", "net_get_alarm_schedule", "读取布防计划", false, AlarmTypeParam(), ctx =>
            {
                int type = ctx.AlarmType;
                NetAlarmSchedule info = NetAlarmSchedule.CreateBlank();
                int code = NetSdk.net_get_alarm_schedule(ctx.Handle, ref type, ref info);
                return Result(code, Dump(info));
            });
            Add("报警", "net_set_alarm_schedule", "把当前布防计划原样写回", false, AlarmTypeParam(), ctx =>
            {
                int type = ctx.AlarmType;
                NetAlarmSchedule info = NetAlarmSchedule.CreateBlank();
                int get = NetSdk.net_get_alarm_schedule(ctx.Handle, ref type, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "");
                type = ctx.AlarmType;
                int set = NetSdk.net_set_alarm_schedule(ctx.Handle, ref type, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_sensor_info", "读取声像传感器信息，不支持则返回错误码", false, ctx =>
            {
                NetAcousticSensorInfo info = NetAcousticSensorInfo.CreateBlank();
                int code = NetSdk.net_get_acoustic_sensor_info(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_get_acoustic_freq_range", "读取频率范围", false, ctx =>
            {
                NetAcousticFreqRange info;
                int code = NetSdk.net_get_acoustic_freq_range(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_freq_range", "把当前频率范围原样写回", false, ctx =>
            {
                NetAcousticFreqRange info;
                int get = NetSdk.net_get_acoustic_freq_range(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_freq_range(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_db_scale", "读取声宽", false, ctx =>
            {
                NetAcousticDbScale info = new NetAcousticDbScale();
                int code = NetSdk.net_get_acoustic_db_scale(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_db_scale", "把当前声宽原样写回", false, ctx =>
            {
                NetAcousticDbScale info = new NetAcousticDbScale();
                int get = NetSdk.net_get_acoustic_db_scale(ctx.Handle, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_db_scale(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_overlay", "读取声像叠加", false, ctx =>
            {
                NetAcousticOverlayInfo info = new NetAcousticOverlayInfo();
                int code = NetSdk.net_get_acoustic_overlay(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_overlay", "把当前声像叠加原样写回", false, ctx =>
            {
                NetAcousticOverlayInfo info = new NetAcousticOverlayInfo();
                int get = NetSdk.net_get_acoustic_overlay(ctx.Handle, ref info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_overlay(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_capture", "抓拍声像图到临时目录", false, AcousticCaptureParam(), ctx =>
            {
                string path = Path.Combine(Path.GetTempPath(), "parkia_acoustic.jpg");
                NetCaptureFile file = NetCaptureFile.FromPath(path);
                int code = NetSdk.net_get_acoustic_capture(ctx.Handle, ctx.CaptureMode, ref file);
                return Result(code, JsonKv("local_file", path));
            });
            Add("声像", "net_get_acoustic_source_alarm", "读取声源报警", false, ctx =>
            {
                NetAcousticSourceAlarm info;
                int code = NetSdk.net_get_acoustic_source_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_source_alarm", "把当前声源报警原样写回", false, ctx =>
            {
                NetAcousticSourceAlarm info;
                int get = NetSdk.net_get_acoustic_source_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_source_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_db_alarm", "读取分贝报警", false, ctx =>
            {
                NetAcousticDbAlarm info;
                int code = NetSdk.net_get_acoustic_db_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_db_alarm", "把当前分贝报警原样写回", false, ctx =>
            {
                NetAcousticDbAlarm info;
                int get = NetSdk.net_get_acoustic_db_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_db_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });
            Add("声像", "net_get_acoustic_freq_alarm", "读取频率报警", false, ctx =>
            {
                NetAcousticFreqAlarm info;
                int code = NetSdk.net_get_acoustic_freq_alarm(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("声像", "net_set_acoustic_freq_alarm", "把当前频率报警原样写回", false, ctx =>
            {
                NetAcousticFreqAlarm info;
                int get = NetSdk.net_get_acoustic_freq_alarm(ctx.Handle, out info);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                int set = NetSdk.net_set_acoustic_freq_alarm(ctx.Handle, ref info);
                return Result(set, Dump(info));
            });

            Add("存储", "net_get_storages", "读取存储卷列表", false, ctx =>
            {
                NetStorageInfo[] list = CreateArray(8, NetStorageInfo.CreateBlank);
                ushort count;
                int code = NetSdk.net_get_storages(ctx.Handle, list, (ushort)list.Length, out count);
                return Result(code, DumpArray(list, count));
            });
            Add("存储", "net_set_storage", "把指定存储卷当前值写回；找不到则写回第一卷。只写入 name、attr、recycling", false, NumberParam("storage_id", "存储卷 ID"), ctx =>
            {
                NetStorageInfo[] list = CreateArray(8, NetStorageInfo.CreateBlank);
                ushort count;
                int get = NetSdk.net_get_storages(ctx.Handle, list, (ushort)list.Length, out count);
                if (get != NetSdk.Ok)
                    return Result(get, "先读取失败");
                if (count == 0)
                    return "{\r\n  \"error\": \"没有存储卷\"\r\n}";
                int index = 0;
                for (int i = 0; i < count; i++)
                {
                    if (list[i].Id == ctx.StorageId)
                    {
                        index = i;
                        break;
                    }
                }
                NetStorageInfo info = list[index];
                int set = NetSdk.net_set_storage(ctx.Handle, info.Id, ref info);
                return Result(set, Dump(info));
            });
            Add("存储", "net_find_alarm_datas", "按报警分页查找数据，返回路径和类型详情", false, SearchAlarmParams(), ctx =>
            {
                var param = new NetSearchAlarmParam
                {
                    AlarmType = (uint)ctx.AlarmType,
                    ChannelId = ctx.ChannelId,
                    PageNo = ctx.PageNo,
                    PageSize = ctx.PageSize
                };
                int capacity = ctx.PageSize == 0 ? 20 : (int)ctx.PageSize;
                NetSearchPage page;
                AlarmSearchItem[] items;
                int code = NetSdk.FindAlarmDatas(ctx.Handle, param, capacity, out page, out items);
                var sb = new StringBuilder();
                sb.Append("{\r\n  \"total\": ").Append(page.Total)
                    .Append(",\r\n  \"page_no\": ").Append(page.PageNo)
                    .Append(",\r\n  \"count\": ").Append(page.Count)
                    .Append(",\r\n  \"datas\": [\r\n");
                if (items != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        AlarmSearchItem item = items[i];
                        if (i > 0)
                            sb.Append(",\r\n");
                        sb.Append("    {\r\n")
                            .Append("      \"alarm_id\": ").Append(item.AlarmId).Append(",\r\n")
                            .Append("      \"channel_id\": ").Append(item.ChannelId).Append(",\r\n")
                            .Append("      \"timestamp_us\": ").Append(item.TimestampUs).Append(",\r\n")
                            .Append("      \"alarm_type\": ").Append(item.AlarmType).Append(",\r\n")
                            .Append("      \"detail\": ").Append(Quote(item.Detail)).Append(",\r\n")
                            .Append("      \"record_path\": ").Append(Quote(item.RecordPath)).Append(",\r\n")
                            .Append("      \"snapshot_path\": ").Append(Quote(item.SnapshotPath)).Append(",\r\n")
                            .Append("      \"data_path\": ").Append(Quote(item.DataPath)).Append("\r\n")
                            .Append("    }");
                    }
                }
                sb.Append("\r\n  ]\r\n}");
                return Result(code, sb.ToString());
            });

            Add("工厂", "net_get_sensor_sn", "读取机芯序列号，通常要连 8088 工厂口", false, ctx =>
            {
                byte[] data = new byte[64];
                int code = NetSdk.net_get_sensor_sn(ctx.Handle, data, (ushort)data.Length);
                return Result(code, JsonKv("sn", NetSdk.CString(data)));
            });
            Add("工厂", "net_get_seal", "读取封印状态", false, ctx =>
            {
                NetSealParam info = NetSealParam.CreateBlank();
                int code = NetSdk.net_get_seal(ctx.Handle, ref info);
                return Result(code, Dump(info));
            });
            Add("工厂", "net_get_preset_info", "读取机芯朝向和挡片方向", false, ctx =>
            {
                NetPresetInfo info;
                int code = NetSdk.net_get_preset_info(ctx.Handle, out info);
                return Result(code, Dump(info));
            });
            Add("工厂", "net_get_sensor_temp", "读取机芯温度", false, ctx =>
            {
                uint temp;
                int code = NetSdk.net_get_sensor_temp(ctx.Handle, out temp);
                return Result(code, JsonKv("temperature", temp));
            });
            Add("工厂", "net_get_license_code", "读取授权码", false, ctx =>
            {
                byte[] data = new byte[128];
                int code = NetSdk.net_get_license_code(ctx.Handle, data, (ushort)data.Length);
                return Result(code, JsonKv("code", NetSdk.CString(data)));
            });
        }

        void Add(string group, string name, string summary, bool dangerous, Func<ApiCallContext, string> run)
        {
            Add(group, name, summary, dangerous, null, run);
        }

        void Add(string group, string name, string summary, bool dangerous, ApiParam[] args, Func<ApiCallContext, string> run)
        {
            string method;
            string path;
            string request;
            string response;
            Describe(name, out method, out path, out request, out response);
            _items.Add(new ApiItem
            {
                Group = group,
                Name = name,
                Summary = summary,
                Dangerous = dangerous,
                Method = method,
                Path = path,
                RequestExample = request,
                ResponseExample = response,
                Params = args ?? new ApiParam[0],
                Run = run
            });
        }

        static ApiParam[] NumberParam(string key, string label, int defaultValue = 0)
        {
            return new[]
            {
                new ApiParam
                {
                    Key = key,
                    Label = label,
                    Kind = ApiParamKind.Number,
                    DefaultValue = defaultValue
                }
            };
        }

        static ApiParam[] EnumParam(string key, string label, int defaultValue, params EnumChoice[] options)
        {
            return new[]
            {
                new ApiParam
                {
                    Key = key,
                    Label = label,
                    Kind = ApiParamKind.Enum,
                    DefaultValue = defaultValue,
                    Options = options
                }
            };
        }

        static ApiParam[] AlarmTypeParam()
        {
            return EnumParam("alarm_type", "报警类型", 1,
                new EnumChoice(1, "热成像温度报警"),
                new EnumChoice(2, "区域入侵报警"),
                new EnumChoice(3, "高温报警"),
                new EnumChoice(4, "温度监测报警"),
                new EnumChoice(5, "声源报警"),
                new EnumChoice(6, "分贝报警"),
                new EnumChoice(7, "频率报警"));
        }

        static ApiParam[] SearchAlarmParams()
        {
            return new[]
            {
                new ApiParam
                {
                    Key = "alarm_type",
                    Label = "报警类型",
                    Kind = ApiParamKind.Enum,
                    DefaultValue = 0,
                    Options = new[]
                    {
                        new EnumChoice(0, "全部"),
                        new EnumChoice(1, "热成像温度报警"),
                        new EnumChoice(2, "区域入侵报警"),
                        new EnumChoice(3, "高温报警"),
                        new EnumChoice(4, "温度监测报警"),
                        new EnumChoice(5, "声源报警"),
                        new EnumChoice(6, "分贝报警"),
                        new EnumChoice(7, "频率报警")
                    }
                },
                new ApiParam
                {
                    Key = "channel_id",
                    Label = "通道",
                    Kind = ApiParamKind.Number,
                    DefaultValue = (int)ChannelIds.Default
                },
                new ApiParam
                {
                    Key = "page_no",
                    Label = "页码",
                    Kind = ApiParamKind.Number,
                    DefaultValue = 0
                },
                new ApiParam
                {
                    Key = "page_size",
                    Label = "每页",
                    Kind = ApiParamKind.Number,
                    DefaultValue = 20
                }
            };
        }

        static ApiParam[] ThermalCaptureParam()
        {
            return EnumParam("capture_mode", "抓拍模式", 1,
                new EnumChoice(0, "纯图像"),
                new EnumChoice(1, "IRtek 图像"));
        }

        static ApiParam[] AcousticCaptureParam()
        {
            return EnumParam("capture_mode", "抓拍模式", 1,
                new EnumChoice(0, "纯图像"),
                new EnumChoice(1, "IRtek 声像图像"));
        }

        static void Describe(string name, out string method, out string path, out string request, out string response)
        {
            method = "GET";
            path = "";
            request = "{}";
            response = "{}";
            switch (name)
            {
                case "net_get_sdk_version":
                    method = "LOCAL";
                    response = "{\r\n  \"major\": 1,\r\n  \"minor\": 0,\r\n  \"patch\": 26,\r\n  \"build\": 0\r\n}";
                    break;
                case "net_get_device_info":
                    path = "/system/device_info";
                    response = ExDeviceInfo;
                    break;
                case "net_set_device_info":
                    method = "PUT";
                    path = "/system/device_info";
                    request = ExDeviceInfo;
                    break;
                case "net_get_time":
                    path = "/system/time";
                    response = ExTime;
                    break;
                case "net_set_time":
                    method = "PUT";
                    path = "/system/time";
                    request = ExTime;
                    response = ExTime;
                    break;
                case "net_get_ntp_server":
                    path = "/system/ntp_server";
                    response = ExNtp;
                    break;
                case "net_set_ntp_server":
                    method = "PUT";
                    path = "/system/ntp_server";
                    request = ExNtp;
                    response = ExNtp;
                    break;
                case "net_get_port":
                    path = "/system/port";
                    response = ExPort;
                    break;
                case "net_set_port":
                    method = "PUT";
                    path = "/system/port";
                    request = ExPort;
                    response = ExPort;
                    break;
                case "net_get_networks":
                    path = "/system/networks";
                    response = "[\r\n" + ExNetwork + "\r\n]";
                    break;
                case "net_get_network":
                    path = "/system/networks/{id}";
                    response = ExNetwork;
                    break;
                case "net_set_network":
                    method = "PUT";
                    path = "/system/networks/{id}";
                    request = ExNetwork;
                    response = ExNetwork;
                    break;
                case "net_get_ftp":
                    path = "/system/ftp";
                    response = ExFtp;
                    break;
                case "net_set_ftp":
                    method = "PUT";
                    path = "/system/ftp";
                    request = ExFtp;
                    response = ExFtp;
                    break;
                case "net_test_ftp":
                    method = "POST";
                    path = "/system/ftp/test";
                    request = ExFtp;
                    break;
                case "net_get_webhook":
                    path = "/system/webhook";
                    response = ExWebhook;
                    break;
                case "net_set_webhook":
                    method = "PUT";
                    path = "/system/webhook";
                    request = ExWebhook;
                    response = ExWebhook;
                    break;
                case "net_get_webhooks":
                    path = "/system/webhooks";
                    response = "[\r\n  { \"url\": \"http://a.example/hook\" },\r\n  { \"url\": \"https://b.example/hook\" }\r\n]";
                    break;
                case "net_set_webhooks":
                    method = "PUT";
                    path = "/system/webhooks";
                    request = "[\r\n  { \"url\": \"http://a.example/hook\" },\r\n  { \"url\": \"https://b.example/hook\" }\r\n]";
                    response = request;
                    break;
                case "net_device_upgrade":
                    method = "POST";
                    path = "/system/device_upgrade";
                    request = "{\r\n  \"path\": \"D:/firmware/upgrade.bin\"\r\n}";
                    break;
                case "net_device_restart":
                    method = "PUT";
                    path = "/system/device_restart";
                    break;
                case "net_device_reset":
                    method = "PUT";
                    path = "/system/device_reset";
                    break;
                case "net_get_users":
                    path = "/security/users";
                    response = "[\r\n  { \"id\": 1, \"user_level\": 0, \"user_name\": \"admin\" }\r\n]";
                    break;
                case "net_get_user":
                    path = "/security/users/{user_id}";
                    response = "{\r\n  \"id\": 1,\r\n  \"user_level\": 0,\r\n  \"user_name\": \"admin\"\r\n}";
                    break;
                case "net_get_streams":
                    path = "/streaming/channels";
                    response = "[\r\n" + ExStream + "\r\n]";
                    break;
                case "net_get_stream":
                    path = "/streaming/channels/{stream_id}";
                    response = ExStream;
                    break;
                case "net_get_stream_status":
                    path = "/streaming/channels/{stream_id}/status";
                    response = "{\r\n  \"id\": 0,\r\n  \"connection_count\": 1\r\n}";
                    break;
                case "net_get_channels":
                    path = "/video/channels";
                    response = "[\r\n  { \"id\": 0, \"name\": \"Thermal\" }\r\n]";
                    break;
                case "net_get_channel":
                    path = "/video/channels/{channel_id}";
                    response = "{\r\n  \"id\": 0,\r\n  \"name\": \"Thermal\"\r\n}";
                    break;
                case "net_set_channel":
                    method = "PUT";
                    path = "/video/channels/{channel_id}";
                    request = "{\r\n  \"id\": 0,\r\n  \"name\": \"Thermal\"\r\n}";
                    response = request;
                    break;
                case "net_get_overlay":
                    path = "/video/channels/{channel_id}/overlay";
                    response = ExOverlay;
                    break;
                case "net_set_overlay":
                    method = "PUT";
                    path = "/video/channels/{channel_id}/overlay";
                    request = ExOverlay;
                    response = ExOverlay;
                    break;
                case "net_get_image_params":
                    path = "/image/channels/{channel_id}/image_params";
                    response = "{\r\n  \"contrast\": 50,\r\n  \"brightness\": 50,\r\n  \"dde_enable\": true,\r\n  \"dde_value\": 30\r\n}";
                    break;
                case "net_set_image_params":
                    method = "PUT";
                    path = "/image/channels/{channel_id}/image_params";
                    request = "{\r\n  \"contrast\": 50,\r\n  \"brightness\": 50,\r\n  \"dde_enable\": true,\r\n  \"dde_value\": 30\r\n}";
                    response = request;
                    break;
                case "net_get_image_flip":
                    path = "/image/channels/{channel_id}/image_flip";
                    response = "{\r\n  \"mode\": 0\r\n}";
                    break;
                case "net_set_image_flip":
                    method = "PUT";
                    path = "/image/channels/{channel_id}/image_flip";
                    request = "{\r\n  \"mode\": 0\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_lens_info":
                    path = "/thermal/lens_info";
                    response = ExLens;
                    break;
                case "net_thermal_calibrate":
                    method = "PUT";
                    path = "/thermal/thermal_calibrate";
                    break;
                case "net_get_thermal_shutter_mode":
                    path = "/thermal/thermal_shutter_mode";
                    response = "{\r\n  \"mode\": 0\r\n}";
                    break;
                case "net_set_thermal_shutter_mode":
                    method = "PUT";
                    path = "/thermal/thermal_shutter_mode";
                    request = "{\r\n  \"mode\": 0\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_shutter_time":
                    path = "/thermal/thermal_shutter_time";
                    response = "{\r\n  \"minutes\": 5\r\n}";
                    break;
                case "net_set_thermal_shutter_time":
                    method = "PUT";
                    path = "/thermal/thermal_shutter_time";
                    request = "{\r\n  \"minutes\": 5\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_resolution":
                    path = "/thermal/thermal_resolution";
                    response = "{\r\n  \"width\": 256,\r\n  \"height\": 192\r\n}";
                    break;
                case "net_get_thermal_range":
                    path = "/thermal/thermal_range";
                    response = "{\r\n  \"range_id\": 0\r\n}";
                    break;
                case "net_set_thermal_range":
                    method = "PUT";
                    path = "/thermal/thermal_range";
                    request = "{\r\n  \"range_id\": 0\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_lut":
                    path = "/thermal/thermal_lut";
                    response = "{\r\n  \"range_id\": 0,\r\n  \"length\": 2,\r\n  \"ad\": [0, 16383],\r\n  \"t\": [253.15, 423.15]\r\n}";
                    break;
                case "net_get_thermal_params":
                    path = "/thermal/thermal_params";
                    response = "{\r\n  \"emissivity\": 0.95,\r\n  \"distance\": 1.0,\r\n  \"refl_temp\": 293.15,\r\n  \"atm_temp\": 293.15,\r\n  \"rel_humidity\": 0.45\r\n}";
                    break;
                case "net_set_thermal_params":
                    method = "PUT";
                    path = "/thermal/thermal_params";
                    request = "{\r\n  \"emissivity\": 0.95,\r\n  \"distance\": 1.0,\r\n  \"refl_temp\": 293.15,\r\n  \"atm_temp\": 293.15,\r\n  \"rel_humidity\": 0.45\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_color_dist":
                    path = "/thermal/color_dist";
                    response = "{\r\n  \"type\": 2\r\n}";
                    break;
                case "net_set_thermal_color_dist":
                    method = "PUT";
                    path = "/thermal/color_dist";
                    request = "{\r\n  \"type\": 2\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_gain":
                    path = "/thermal/gain";
                    response = "{\r\n  \"mode\": 2,\r\n  \"max\": 40.0,\r\n  \"min\": 20.0,\r\n  \"fixed_span\": 0,\r\n  \"span\": 2.0\r\n}";
                    break;
                case "net_set_thermal_gain":
                    method = "PUT";
                    path = "/thermal/gain";
                    request = "{\r\n  \"mode\": 2,\r\n  \"max\": 40.0,\r\n  \"min\": 20.0,\r\n  \"fixed_span\": 0,\r\n  \"span\": 2.0\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_preset_palettes":
                    path = "/thermal/preset_palettes";
                    response = "[\r\n  { \"ref_no\": 1, \"name\": \"Iron\" }\r\n]";
                    break;
                case "net_get_thermal_palette":
                    path = "/thermal/palette";
                    response = "{\r\n  \"ref_no\": 1,\r\n  \"colors\": [16711680, 65280],\r\n  \"color_count\": 256,\r\n  \"inverted\": false\r\n}";
                    break;
                case "net_set_thermal_palette":
                    method = "PUT";
                    path = "/thermal/palette";
                    request = "{\r\n  \"ref_no\": 1,\r\n  \"colors\": [16711680, 65280],\r\n  \"color_count\": 256,\r\n  \"inverted\": false\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_overlay":
                    path = "/thermal/thermal_overlay";
                    response = ExThermalOverlay;
                    break;
                case "net_set_thermal_overlay":
                    method = "PUT";
                    path = "/thermal/thermal_overlay";
                    request = ExThermalOverlay;
                    response = ExThermalOverlay;
                    break;
                case "net_get_thermal_unit":
                    path = "/thermal/thermal_unit";
                    response = "{\r\n  \"temp_unit\": 0,\r\n  \"distance_unit\": 0\r\n}";
                    break;
                case "net_set_thermal_unit":
                    method = "PUT";
                    path = "/thermal/thermal_unit";
                    request = "{\r\n  \"temp_unit\": 0,\r\n  \"distance_unit\": 0\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_capture":
                    path = "/thermal/capture/{mode}";
                    response = "{\r\n  \"local_file\": \"C:/Temp/parkia_thermal.jpg\"\r\n}";
                    break;
                case "net_get_thermal_markers":
                    path = "/thermal/marker";
                    response = ExMarkers;
                    break;
                case "net_set_thermal_markers":
                    method = "PUT";
                    path = "/thermal/marker";
                    request = ExMarkers;
                    response = ExMarkers;
                    break;
                case "net_get_thermal_temp_alarms":
                    path = "/thermal/temp/alarm";
                    response = ExTempAlarms;
                    break;
                case "net_set_thermal_temp_alarms":
                    method = "PUT";
                    path = "/thermal/temp/alarm";
                    request = ExTempAlarms;
                    response = ExTempAlarms;
                    break;
                case "net_get_thermal_temp_alarm_interval":
                    path = "/thermal/temp/alarm/interval";
                    response = "{\r\n  \"interval\": 10\r\n}";
                    break;
                case "net_set_thermal_temp_alarm_interval":
                    method = "PUT";
                    path = "/thermal/temp/alarm/interval";
                    request = "{\r\n  \"interval\": 10\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_intrusion_alarm":
                    path = "/thermal/intrusion/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"no_repeat_minutes\": 5,\r\n  \"sensitivity\": 1,\r\n  \"record_duration_sec\": 30,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    break;
                case "net_set_thermal_intrusion_alarm":
                    method = "PUT";
                    path = "/thermal/intrusion/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"no_repeat_minutes\": 5,\r\n  \"sensitivity\": 1,\r\n  \"record_duration_sec\": 30,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_high_temp_alarm":
                    path = "/thermal/high_temp/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"no_repeat_minutes\": 3,\r\n  \"alarm_temp\": 38.5,\r\n  \"record_duration_sec\": 30,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    break;
                case "net_set_thermal_high_temp_alarm":
                    method = "PUT";
                    path = "/thermal/high_temp/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"no_repeat_minutes\": 3,\r\n  \"alarm_temp\": 38.5,\r\n  \"record_duration_sec\": 30,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    response = request;
                    break;
                case "net_get_thermal_temp_detect_alarm":
                    path = "/thermal/temp_detect/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"sensitivity\": 1,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    break;
                case "net_set_thermal_temp_detect_alarm":
                    method = "PUT";
                    path = "/thermal/temp_detect/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"sensitivity\": 1,\r\n  \"region\": " + ExAlarmRegion + "\r\n}";
                    response = request;
                    break;
                case "net_get_alarm_trigger":
                    path = "/alarm/triggers/{type}";
                    response = ExTrigger;
                    break;
                case "net_set_alarm_trigger":
                    method = "PUT";
                    path = "/alarm/triggers/{type}";
                    request = ExTrigger;
                    response = ExTrigger;
                    break;
                case "net_get_alarm_schedule":
                    path = "/alarm/schedule/{type}";
                    response = ExSchedule;
                    break;
                case "net_set_alarm_schedule":
                    method = "PUT";
                    path = "/alarm/schedule/{type}";
                    request = ExSchedule;
                    response = ExSchedule;
                    break;
                case "net_get_acoustic_sensor_info":
                    path = "/acoustic/sensor_info";
                    response = "{\r\n  \"firmware_version\": \"1.0.0\",\r\n  \"serial_no\": \"AC001\",\r\n  \"sample_rate\": 48000,\r\n  \"mic_count\": 64,\r\n  \"ref_mic_index\": 0,\r\n  \"amplitude\": 1.0\r\n}";
                    break;
                case "net_get_acoustic_freq_range":
                    path = "/acoustic/freq_range";
                    response = "{\r\n  \"min\": 2000.0,\r\n  \"max\": 40000.0\r\n}";
                    break;
                case "net_set_acoustic_freq_range":
                    method = "PUT";
                    path = "/acoustic/freq_range";
                    request = "{\r\n  \"min\": 2000.0,\r\n  \"max\": 40000.0\r\n}";
                    response = request;
                    break;
                case "net_get_acoustic_db_scale":
                    path = "/acoustic/db_scale";
                    response = "{\r\n  \"mode\": 1,\r\n  \"max\": 80.0,\r\n  \"min\": 20.0,\r\n  \"fixed_span\": 0,\r\n  \"span\": 10.0,\r\n  \"sensitivity\": 0.5,\r\n  \"source_size\": 8,\r\n  \"limit\": 20.0\r\n}";
                    break;
                case "net_set_acoustic_db_scale":
                    method = "PUT";
                    path = "/acoustic/db_scale";
                    request = "{\r\n  \"mode\": 1,\r\n  \"max\": 80.0,\r\n  \"min\": 20.0,\r\n  \"fixed_span\": 0,\r\n  \"span\": 10.0,\r\n  \"sensitivity\": 0.5,\r\n  \"source_size\": 8,\r\n  \"limit\": 20.0\r\n}";
                    response = request;
                    break;
                case "net_get_acoustic_overlay":
                    path = "/acoustic/acoustic_overlay";
                    response = ExAcousticOverlay;
                    break;
                case "net_set_acoustic_overlay":
                    method = "PUT";
                    path = "/acoustic/acoustic_overlay";
                    request = ExAcousticOverlay;
                    response = ExAcousticOverlay;
                    break;
                case "net_get_acoustic_capture":
                    path = "/acoustic/capture/{mode}";
                    response = "{\r\n  \"local_file\": \"C:/Temp/parkia_acoustic.jpg\"\r\n}";
                    break;
                case "net_get_acoustic_source_alarm":
                    path = "/acoustic/source/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"duration\": 3,\r\n  \"tolerance_pixels\": 8,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    break;
                case "net_set_acoustic_source_alarm":
                    method = "PUT";
                    path = "/acoustic/source/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"duration\": 3,\r\n  \"tolerance_pixels\": 8,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    response = request;
                    break;
                case "net_get_acoustic_db_alarm":
                    path = "/acoustic/db/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"db_threshold\": 65.0,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    break;
                case "net_set_acoustic_db_alarm":
                    method = "PUT";
                    path = "/acoustic/db/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"db_threshold\": 65.0,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    response = request;
                    break;
                case "net_get_acoustic_freq_alarm":
                    path = "/acoustic/freq/alarm";
                    response = "{\r\n  \"enabled\": true,\r\n  \"frequency\": 8000.0,\r\n  \"db_threshold\": 65.0,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    break;
                case "net_set_acoustic_freq_alarm":
                    method = "PUT";
                    path = "/acoustic/freq/alarm";
                    request = "{\r\n  \"enabled\": true,\r\n  \"frequency\": 8000.0,\r\n  \"db_threshold\": 65.0,\r\n  \"no_repeat_minutes\": 5\r\n}";
                    response = request;
                    break;
                case "net_get_storages":
                    path = "/storage/info";
                    response = "[\r\n" + ExStorage + "\r\n]";
                    break;
                case "net_set_storage":
                    method = "PUT";
                    path = "/storage/info/{storage_id}";
                    request = ExStorage;
                    response = ExStorage;
                    break;
                case "net_find_alarm_datas":
                    path = "/storage/alarm_datas";
                    request = "{\r\n  \"alarm_type\": 0,\r\n  \"channel_id\": 101,\r\n  \"page_no\": 0,\r\n  \"page_size\": 20\r\n}";
                    response = "{\r\n  \"total\": 1,\r\n  \"page_no\": 0,\r\n  \"count\": 1,\r\n  \"datas\": [\r\n    {\r\n      \"alarm_id\": 12,\r\n      \"channel_id\": 1,\r\n      \"timestamp_us\": 1760803200000000,\r\n      \"alarm_type\": 2,\r\n      \"record_path\": \"\",\r\n      \"snapshot_path\": \"\",\r\n      \"data_path\": \"\"\r\n    }\r\n  ]\r\n}";
                    break;
                case "net_get_sensor_sn":
                    path = "/sensor/sn";
                    response = "{\r\n  \"sn\": \"SN20260001\"\r\n}";
                    break;
                case "net_get_seal":
                    path = "/system/seal";
                    response = "{\r\n  \"sealed\": true,\r\n  \"path\": \"\"\r\n}";
                    break;
                case "net_get_preset_info":
                    path = "/sensor/preset";
                    response = "{\r\n  \"mode\": { \"mode\": 0 },\r\n  \"direction\": { \"inverted\": false }\r\n}";
                    break;
                case "net_get_sensor_temp":
                    path = "/sensor/temp";
                    response = "{\r\n  \"temperature\": 31245\r\n}";
                    break;
                case "net_get_license_code":
                    path = "/system/license/code";
                    response = "{\r\n  \"code\": \"LICENSE-CODE\"\r\n}";
                    break;
            }
        }

        const string ExDeviceInfo = "{\r\n  \"name\": \"IR-Cam-01\",\r\n  \"model\": \"IR-640\",\r\n  \"spec\": \"Fotric IR-640#L25\",\r\n  \"serial_no\": \"1234567890\",\r\n  \"id\": \"device-001\",\r\n  \"brand\": \"IRtek\",\r\n  \"company\": \"Volga\",\r\n  \"firmware_name\": \"main\",\r\n  \"firmware_version\": \"1.0.26\"\r\n}";
        const string ExTime = "{\r\n  \"time_mode\": 1,\r\n  \"local_time\": \"2026-05-20T10:09:00+08:00\",\r\n  \"time_zone\": \"UTC+08:00\"\r\n}";
        const string ExNtp = "{\r\n  \"host_name\": \"pool.ntp.org\",\r\n  \"port_no\": 123,\r\n  \"syn_interval\": 1440\r\n}";
        const string ExPort = "{\r\n  \"http_port_no\": 80,\r\n  \"rtsp_port_no\": 554,\r\n  \"rtp_port_no\": 8001\r\n}";
        const string ExNetwork = "{\r\n  \"id\": 1,\r\n  \"name\": \"eth0\",\r\n  \"dhcp\": true,\r\n  \"ip\": \"192.168.10.177\",\r\n  \"netmask\": \"255.255.255.0\",\r\n  \"dns\": [\"192.168.10.11\", \"192.168.10.12\"],\r\n  \"gateway\": [\"192.168.10.1\"]\r\n}";
        const string ExFtp = "{\r\n  \"use_ssl\": false,\r\n  \"ip\": \"192.168.10.20\",\r\n  \"port_no\": 21,\r\n  \"user_name\": \"ftp_user\",\r\n  \"pwd\": \"password\",\r\n  \"enable_annoy\": false,\r\n  \"path_template\": \"{DEVICE_NAME}/{yyyyMMddHHmmssfff}.jpg\"\r\n}";
        const string ExWebhook = "{\r\n  \"url\": \"https://a.example/hook\"\r\n}";
        const string ExStream = "{\r\n  \"id\": 0,\r\n  \"bit_rate\": 2048,\r\n  \"codec_type\": \"H264\",\r\n  \"resolution_width\": 256,\r\n  \"resolution_height\": 192\r\n}";
        const string ExOverlay = "{\r\n  \"channel_name_overlay\": { \"align\": 0, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"time_overlay\": { \"align\": 1, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"custom_overlay\": { \"align\": 2, \"enabled\": false, \"mode\": 0, \"x\": 0, \"y\": 0, \"text\": \"Parkia\" }\r\n}";
        const string ExLens = "{\r\n  \"name\": \"L50\",\r\n  \"focal\": 23.4,\r\n  \"vfov\": 50.0,\r\n  \"hfov\": 50.0,\r\n  \"band\": \"8-14um\",\r\n  \"range_map\": [\r\n    { \"id\": 0, \"range\": { \"min\": 253.15, \"max\": 423.15 } }\r\n  ]\r\n}";
        const string ExThermalOverlay = "{\r\n  \"unit_overlay\": { \"align\": 0, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"plt_overlay\": { \"align\": 1, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"global_overlay\": { \"align\": 2, \"enabled\": true, \"follow\": true, \"max_enabled\": true, \"min_enabled\": true, \"avg_enabled\": false }\r\n}";
        const string ExMarkers = "[\r\n  {\r\n    \"id\": 1,\r\n    \"type\": 1,\r\n    \"name\": \"Spot1\",\r\n    \"local_params\": 0,\r\n    \"refl_temp\": 293.15,\r\n    \"distance\": 1.0,\r\n    \"emissivity\": 0.95,\r\n    \"points\": [{ \"x\": 80, \"y\": 60 }]\r\n  }\r\n]";
        const string ExTempAlarms = "[\r\n  {\r\n    \"marker_name\": \"Spot1\",\r\n    \"enabled\": true,\r\n    \"alarm_type\": 1,\r\n    \"alarm_temp\": 80.0,\r\n    \"alarm_delay_time\": 0,\r\n    \"warning_temp\": 70.0,\r\n    \"warning_delay_time\": 0,\r\n    \"alert_temp\": 60.0,\r\n    \"alert_delay_time\": 0\r\n  }\r\n]";
        const string ExAlarmRegion = "[\r\n    {\r\n      \"id\": 1,\r\n      \"points\": [\r\n        { \"x\": 10, \"y\": 20 },\r\n        { \"x\": 200, \"y\": 20 },\r\n        { \"x\": 200, \"y\": 150 }\r\n      ]\r\n    }\r\n  ]";
        const string ExTrigger = "{\r\n  \"trigger_ftp_enabled\": true,\r\n  \"trigger_webhook_enabled\": false\r\n}";
        const string ExSchedule = "{\r\n  \"days\": [\r\n    {\r\n      \"weekday\": 1,\r\n      \"periods\": [\r\n        { \"start_time\": \"08:00:00\", \"end_time\": \"12:00:00\" },\r\n        { \"start_time\": \"14:00:00\", \"end_time\": \"18:00:00\" }\r\n      ]\r\n    },\r\n    {\r\n      \"weekday\": 2,\r\n      \"periods\": [\r\n        { \"start_time\": \"08:00:00\", \"end_time\": \"18:00:00\" }\r\n      ]\r\n    }\r\n  ]\r\n}";
        const string ExAcousticOverlay = "{\r\n  \"palette_overlay\": { \"align\": 0, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"spectrum_overlay\": { \"align\": 1, \"enabled\": true, \"mode\": 0, \"x\": 0, \"y\": 0 },\r\n  \"spl_overlay\": { \"align\": 2, \"enabled\": true, \"max_db_enabled\": true, \"max_freq_enabled\": true, \"x\": 0, \"y\": 0 }\r\n}";
        const string ExStorage = "{\r\n  \"id\": 0,\r\n  \"name\": \"hdd1\",\r\n  \"capacity_mb\": 102400,\r\n  \"free_mb\": 51200,\r\n  \"status\": 0,\r\n  \"attr\": 0,\r\n  \"recycling\": true\r\n}";

        static T[] CreateArray<T>(int length, Func<T> create)
        {
            T[] list = new T[length];
            for (int i = 0; i < length; i++)
                list[i] = create();
            return list;
        }

        static string RoundTrip(Func<Tuple<int, object>> getter, Func<object, int> setter)
        {
            Tuple<int, object> got = getter();
            if (got.Item1 != NetSdk.Ok)
                return Result(got.Item1, "读取失败");
            int set = setter(got.Item2);
            return Result(set, Dump(got.Item2));
        }

        static string Result(int code, string body)
        {
            if (code != NetSdk.Ok)
                return "{\r\n  \"error\": " + Quote(NetSdk.ErrorText(code)) + ",\r\n  \"code\": " + code + "\r\n}";
            if (string.IsNullOrEmpty(body))
                return "{}";
            string trimmed = body.Trim();
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
                return trimmed;
            return "{\r\n  \"value\": " + Quote(trimmed) + "\r\n}";
        }

        static string JsonKv(string key, object value)
        {
            return "{\r\n  " + Quote(key) + ": " + JsonToken(value) + "\r\n}";
        }

        static string JsonToken(object value)
        {
            if (value == null)
                return "null";
            if (value is string)
                return Quote((string)value);
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is float || value is double)
                return Convert.ToSingle(value).ToString("0.######", CultureInfo.InvariantCulture);
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        static string Quote(string text)
        {
            if (text == null)
                text = "";
            var sb = new StringBuilder();
            sb.Append('"');
            foreach (char c in text)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\\\"); break;
                    case '"': sb.Append("\\\""); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20)
                            sb.AppendFormat("\\u{0:x4}", (int)c);
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        static string IndentJson(string json, int levels)
        {
            string pad = new string(' ', levels * 2);
            string[] lines = json.Replace("\r\n", "\n").Split('\n');
            var sb = new StringBuilder();
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                    sb.Append("\r\n");
                sb.Append(pad);
                sb.Append(lines[i]);
            }
            return sb.ToString();
        }

        static string DumpArray<T>(T[] list, ushort count)
        {
            int shown = list == null ? 0 : Math.Min((int)count, list.Length);
            var sb = new StringBuilder();
            WriteArray(sb, list, shown, 0);
            return sb.ToString();
        }

        static string Dump(object value)
        {
            var sb = new StringBuilder();
            WriteJson(sb, value, 0);
            return sb.ToString();
        }

        static void WriteJson(StringBuilder sb, object value, int indent)
        {
            if (value == null)
            {
                sb.Append("null");
                return;
            }

            Type type = value.GetType();
            if (type == typeof(string))
            {
                sb.Append(Quote((string)value));
                return;
            }
            if (type == typeof(bool))
            {
                sb.Append((bool)value ? "true" : "false");
                return;
            }
            if (type == typeof(float) || type == typeof(double))
            {
                sb.Append(Convert.ToSingle(value).ToString("0.######", CultureInfo.InvariantCulture));
                return;
            }
            if (type == typeof(byte[]))
            {
                sb.Append(Quote(NetSdk.CString((byte[])value)));
                return;
            }
            if (type.IsPrimitive || type.IsEnum)
            {
                sb.Append(Convert.ToString(value, CultureInfo.InvariantCulture));
                return;
            }
            if (type.IsArray)
            {
                WriteArray(sb, (Array)value, -1, indent);
                return;
            }

            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            sb.AppendLine("{");
            bool first = true;
            foreach (FieldInfo field in fields)
            {
                if (SkipCountField(field.Name, fields))
                    continue;
                if (first)
                    first = false;
                else
                    sb.AppendLine(",");
                Pad(sb, indent + 1);
                sb.Append(Quote(Snake(field.Name)));
                sb.Append(": ");
                object fieldValue = field.GetValue(value);
                if (field.FieldType == typeof(byte[]) && IsPackedStrings(field.Name))
                {
                    WritePackedStrings(sb, (byte[])fieldValue, RelatedCount(value, field.Name, fields), indent + 1);
                }
                else if (field.FieldType.IsArray && field.FieldType != typeof(byte[]))
                {
                    WriteArray(sb, (Array)fieldValue, RelatedCount(value, field.Name, fields), indent + 1);
                }
                else if (LooksBool(field.Name, field.FieldType))
                {
                    sb.Append(Convert.ToInt32(fieldValue) != 0 ? "true" : "false");
                }
                else
                {
                    WriteJson(sb, fieldValue, indent + 1);
                }
            }
            sb.AppendLine();
            Pad(sb, indent);
            sb.Append("}");
        }

        static void WriteArray(StringBuilder sb, Array array, int count, int indent)
        {
            if (array == null)
            {
                sb.Append("[]");
                return;
            }
            int n = count >= 0 ? Math.Min(count, array.Length) : array.Length;
            bool primitive = n > 0 && array.GetValue(0) != null && array.GetValue(0).GetType().IsPrimitive;
            int shown = primitive && n > 64 ? 64 : n;
            if (primitive)
            {
                sb.Append("[");
                for (int i = 0; i < shown; i++)
                {
                    if (i > 0)
                        sb.Append(", ");
                    WriteJson(sb, array.GetValue(i), indent);
                }
                sb.Append("]");
                return;
            }
            sb.AppendLine("[");
            for (int i = 0; i < shown; i++)
            {
                if (i > 0)
                    sb.AppendLine(",");
                Pad(sb, indent + 1);
                WriteJson(sb, array.GetValue(i), indent + 1);
            }
            if (shown > 0)
                sb.AppendLine();
            Pad(sb, indent);
            sb.Append("]");
        }

        static void WritePackedStrings(StringBuilder sb, byte[] bytes, int count, int indent)
        {
            if (bytes == null)
            {
                sb.Append("[]");
                return;
            }
            int chunk = NetSdk.Text64;
            int max = bytes.Length / chunk;
            int n = count >= 0 ? Math.Min(count, max) : max;
            sb.Append("[");
            for (int i = 0; i < n; i++)
            {
                if (i > 0)
                    sb.Append(", ");
                byte[] slice = new byte[chunk];
                Buffer.BlockCopy(bytes, i * chunk, slice, 0, chunk);
                sb.Append(Quote(NetSdk.CString(slice)));
            }
            sb.Append("]");
        }

        static int RelatedCount(object parent, string fieldName, FieldInfo[] fields)
        {
            string[] names;
            if (fieldName == "Colors")
                names = new[] { "ColorCount" };
            else if (fieldName == "RangeMap")
                names = new[] { "RangesCount" };
            else if (fieldName == "Ad" || fieldName == "T")
                names = new[] { "Length" };
            else
                names = new[] { fieldName + "Count" };
            foreach (string name in names)
            {
                foreach (FieldInfo field in fields)
                {
                    if (field.Name != name)
                        continue;
                    object value = field.GetValue(parent);
                    if (value != null)
                        return Convert.ToInt32(value);
                }
            }
            return -1;
        }

        static bool SkipCountField(string name, FieldInfo[] fields)
        {
            if (name == "Length")
                return false;
            if (!name.EndsWith("Count") || name.Length <= 5)
                return false;
            string stem = name.Substring(0, name.Length - 5);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == stem || field.Name == stem + "s")
                    return true;
            }
            return false;
        }

        static bool IsPackedStrings(string name)
        {
            return name == "Dns" || name == "Gateway";
        }

        static bool LooksBool(string name, Type type)
        {
            if (type != typeof(byte) && type != typeof(bool))
                return false;
            string n = name.ToLowerInvariant();
            return n == "enabled" || n == "dhcp" || n == "inverted" || n == "sealed" || n == "follow"
                || n == "recycling" || n == "ddeenable" || n.EndsWith("enabled") || n.Contains("ssl") || n.Contains("annoy");
        }

        static string Snake(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;
            var sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (char.IsUpper(c) && i > 0)
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            return sb.ToString();
        }

        static void Pad(StringBuilder sb, int indent)
        {
            sb.Append(' ', indent * 2);
        }
    }
}
