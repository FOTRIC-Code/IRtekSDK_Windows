using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IRtekHLAPI
{
    internal sealed class HlApiSession : IDisposable
    {
        private readonly object _sync = new object();
        private HttpClient _http;
        private bool _disposed;

        public string BaseUrl { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public uint TimeoutMs { get; private set; }

        // Keep thermal snapshot buffers alive until next get_thermal_data call.
        public byte[] ThermalPixelBytes;
        public byte[] ThermalLutTempBytes;
        public byte[] ThermalLutAdBytes;
        public GCHandleHolder ThermalPixelPin;
        public GCHandleHolder ThermalLutTempPin;
        public GCHandleHolder ThermalLutAdPin;

        public static HlApiSession Create(string ip, ushort port, string user, string password, uint timeoutMs)
        {
            if (string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("ip required", "ip");

            ushort usePort = port > 0 ? port : (ushort)80;
            string baseUrl;
            if (ip.IndexOf("://", StringComparison.Ordinal) >= 0)
                baseUrl = ip.TrimEnd('/') + ":" + usePort;
            else
                baseUrl = "http://" + ip + ":" + usePort;

            var session = new HlApiSession
            {
                BaseUrl = baseUrl.TrimEnd('/'),
                Username = user ?? string.Empty,
                Password = password ?? string.Empty,
                TimeoutMs = timeoutMs > 0 ? timeoutMs : 5000u
            };
            session.RebuildHttpClient();
            return session;
        }

        private void RebuildHttpClient()
        {
            lock (_sync)
            {
                if (_http != null)
                {
                    _http.Dispose();
                    _http = null;
                }

                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                _http = new HttpClient(handler)
                {
                    BaseAddress = new Uri(BaseUrl + "/"),
                    Timeout = TimeSpan.FromMilliseconds(TimeoutMs)
                };
                // Embedded device HTTP stacks often RST on Expect: 100-continue.
                _http.DefaultRequestHeaders.ExpectContinue = false;
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password));
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", token);
                _http.DefaultRequestHeaders.Accept.Clear();
                _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            }
        }

        public NetSdkNative.NetErrorCode GetJson(string path, out object data)
        {
            data = null;
            return SendJson(HttpMethod.Get, path, null, out data);
        }

        public NetSdkNative.NetErrorCode PutJson(string path, object body, out object data)
        {
            return SendJson(HttpMethod.Put, path, body, out data);
        }

        public NetSdkNative.NetErrorCode PutJson(string path, object body)
        {
            object data;
            return PutJson(path, body, out data);
        }

        public NetSdkNative.NetErrorCode PostJson(string path, object body, out object data)
        {
            return SendJson(HttpMethod.Post, path, body, out data);
        }

        public NetSdkNative.NetErrorCode PostJson(string path, object body)
        {
            object data;
            return PostJson(path, body, out data);
        }

        public NetSdkNative.NetErrorCode Delete(string path)
        {
            object data;
            return SendJson(HttpMethod.Delete, path, null, out data);
        }

        public NetSdkNative.NetErrorCode GetBinary(string path, out byte[] bytes)
        {
            bytes = null;
            HttpClient http;
            lock (_sync)
            {
                if (_disposed || _http == null)
                    return NetSdkNative.NetErrorCode.NotInitialized;
                http = _http;
            }

            try
            {
                // Avoid the session default Accept: application/json so device returns raw image/octet-stream.
                using (var request = new HttpRequestMessage(HttpMethod.Get, TrimPath(path)))
                {
                    request.Headers.Accept.Clear();
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                    using (var response = http.SendAsync(request).Result)
                    {
                        if (response.StatusCode == HttpStatusCode.Unauthorized)
                            return NetSdkNative.NetErrorCode.AuthFailed;
                        if (!response.IsSuccessStatusCode)
                            return NetSdkNative.NetErrorCode.HttpError;
                        bytes = response.Content.ReadAsByteArrayAsync().Result;
                        if (bytes == null || bytes.Length == 0)
                            return NetSdkNative.NetErrorCode.HttpError;
                        return NetSdkNative.NetErrorCode.OK;
                    }
                }
            }
            catch (AggregateException ae)
            {
                if (IsTimeout(ae))
                    return NetSdkNative.NetErrorCode.Timeout;
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch (Exception)
            {
                return NetSdkNative.NetErrorCode.HttpError;
            }
        }

        public NetSdkNative.NetErrorCode UploadFile(string path, string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                return NetSdkNative.NetErrorCode.InvalidParam;

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(filePath);
            }
            catch
            {
                return NetSdkNative.NetErrorCode.InvalidParam;
            }

            HttpClient http;
            lock (_sync)
            {
                if (_disposed || _http == null)
                    return NetSdkNative.NetErrorCode.NotInitialized;
                http = _http;
            }

            try
            {
                // Device HTTP (cpp-httplib) rejects .NET MultipartFormDataContent:
                // quoted boundary + filename* reset the connection. Send the same
                // unquoted multipart the native SDK uses.
                string fileName = Path.GetFileName(filePath);
                if (string.IsNullOrEmpty(fileName))
                    fileName = "firmware.bin";
                fileName = fileName.Replace("\"", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

                string boundary = "----IRtekHLAPI" + Guid.NewGuid().ToString("N");
                byte[] payload = BuildHttplibMultipart(bytes, fileName, boundary);
                string url = BaseUrl + "/" + TrimPath(path);
                int timeoutMs = TimeoutMs > 120000u ? (int)TimeoutMs : 120000;

                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Timeout = timeoutMs;
                req.ReadWriteTimeout = timeoutMs;
                req.KeepAlive = false;
                req.ServicePoint.Expect100Continue = false;
                req.Headers[HttpRequestHeader.Authorization] =
                    "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password));
                req.ContentType = "multipart/form-data; boundary=" + boundary;
                req.ContentLength = payload.Length;
                using (var stream = req.GetRequestStream())
                    stream.Write(payload, 0, payload.Length);

                using (var response = (HttpWebResponse)req.GetResponse())
                    return ReadUploadResponse(response);
            }
            catch (WebException wex)
            {
                var webResponse = wex.Response as HttpWebResponse;
                if (webResponse != null)
                {
                    if (webResponse.StatusCode == HttpStatusCode.Unauthorized)
                        return NetSdkNative.NetErrorCode.AuthFailed;
                    return ReadUploadResponse(webResponse);
                }
                if (wex.Status == WebExceptionStatus.Timeout)
                    return NetSdkNative.NetErrorCode.Timeout;
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch (AggregateException ae)
            {
                if (IsTimeout(ae))
                    return NetSdkNative.NetErrorCode.Timeout;
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch
            {
                return NetSdkNative.NetErrorCode.HttpError;
            }
        }

        private static byte[] BuildHttplibMultipart(byte[] fileBytes, string fileName, string boundary)
        {
            byte[] prelude = Encoding.ASCII.GetBytes(
                "--" + boundary + "\r\n" +
                "Content-Disposition: form-data; name=\"file\"; filename=\"" + fileName + "\"\r\n" +
                "Content-Type: application/octet-stream\r\n" +
                "\r\n");
            byte[] epilogue = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            var payload = new byte[prelude.Length + fileBytes.Length + epilogue.Length];
            Buffer.BlockCopy(prelude, 0, payload, 0, prelude.Length);
            Buffer.BlockCopy(fileBytes, 0, payload, prelude.Length, fileBytes.Length);
            Buffer.BlockCopy(epilogue, 0, payload, prelude.Length + fileBytes.Length, epilogue.Length);
            return payload;
        }

        private static NetSdkNative.NetErrorCode ReadUploadResponse(HttpWebResponse response)
        {
            string body;
            using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                body = reader.ReadToEnd();

            if ((int)response.StatusCode == 401)
                return NetSdkNative.NetErrorCode.AuthFailed;
            if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                return NetSdkNative.NetErrorCode.HttpError;

            int code;
            string message;
            object data;
            if (!HlApiJson.TryParseEnvelope(body, out code, out message, out data))
                return NetSdkNative.NetErrorCode.OK;
            return code == 0 ? NetSdkNative.NetErrorCode.OK : NetSdkNative.NetErrorCode.HttpError;
        }

        private NetSdkNative.NetErrorCode SendJson(HttpMethod method, string path, object body, out object data)
        {
            data = null;
            HttpClient http;
            lock (_sync)
            {
                if (_disposed || _http == null)
                    return NetSdkNative.NetErrorCode.NotInitialized;
                http = _http;
            }

            try
            {
                HttpContent content = null;
                if (body != null && method != HttpMethod.Get && method != HttpMethod.Delete)
                {
                    string json = HlApiJson.Serialize(body);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                HttpRequestMessage request = new HttpRequestMessage(method, TrimPath(path));
                if (content != null)
                    request.Content = content;

                using (request)
                using (var response = http.SendAsync(request).Result)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        return NetSdkNative.NetErrorCode.AuthFailed;

                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    if (!response.IsSuccessStatusCode)
                        return NetSdkNative.NetErrorCode.HttpError;

                    int code;
                    string message;
                    object payload;
                    if (!HlApiJson.TryParseEnvelope(responseBody, out code, out message, out payload))
                        return NetSdkNative.NetErrorCode.HttpError;

                    data = payload;
                    return code == 0 ? NetSdkNative.NetErrorCode.OK : NetSdkNative.NetErrorCode.HttpError;
                }
            }
            catch (AggregateException ae)
            {
                if (IsTimeout(ae))
                    return NetSdkNative.NetErrorCode.Timeout;
                return NetSdkNative.NetErrorCode.HttpError;
            }
            catch
            {
                return NetSdkNative.NetErrorCode.HttpError;
            }
        }

        private static string TrimPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            return path.StartsWith("/") ? path.Substring(1) : path;
        }

        private static bool IsTimeout(AggregateException ae)
        {
            foreach (var ex in ae.Flatten().InnerExceptions)
            {
                if (ex is TaskCanceledException || ex is TimeoutException)
                    return true;
            }
            return false;
        }

        public void ClearThermalPins()
        {
            if (ThermalPixelPin != null) { ThermalPixelPin.Dispose(); ThermalPixelPin = null; }
            if (ThermalLutTempPin != null) { ThermalLutTempPin.Dispose(); ThermalLutTempPin = null; }
            if (ThermalLutAdPin != null) { ThermalLutAdPin.Dispose(); ThermalLutAdPin = null; }
            ThermalPixelBytes = null;
            ThermalLutTempBytes = null;
            ThermalLutAdBytes = null;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            _disposed = true;
            ClearThermalPins();
            lock (_sync)
            {
                if (_http != null)
                {
                    _http.Dispose();
                    _http = null;
                }
            }
        }
    }

    internal sealed class GCHandleHolder : IDisposable
    {
        private System.Runtime.InteropServices.GCHandle _handle;
        private bool _allocated;

        public GCHandleHolder(object target)
        {
            _handle = System.Runtime.InteropServices.GCHandle.Alloc(target, System.Runtime.InteropServices.GCHandleType.Pinned);
            _allocated = true;
        }

        public IntPtr AddrOfPinnedObject()
        {
            return _handle.AddrOfPinnedObject();
        }

        public void Dispose()
        {
            if (_allocated)
            {
                _handle.Free();
                _allocated = false;
            }
        }
    }

}
