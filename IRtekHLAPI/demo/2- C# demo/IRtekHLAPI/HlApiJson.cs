using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web.Script.Serialization;

namespace IRtekHLAPI
{
    internal static class HlApiJson
    {
        private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer
        {
            MaxJsonLength = int.MaxValue
        };

        public static string Serialize(object value)
        {
            return Serializer.Serialize(value);
        }

        public static object DeserializeObject(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;
            return Serializer.DeserializeObject(json);
        }

        public static bool TryParseEnvelope(string body, out int code, out string message, out object data)
        {
            code = -999;
            message = string.Empty;
            data = null;
            if (string.IsNullOrWhiteSpace(body))
                return false;

            object rootObj;
            try
            {
                rootObj = DeserializeObject(body);
            }
            catch
            {
                return false;
            }

            var root = rootObj as Dictionary<string, object>;
            if (root == null)
                return false;

            code = ToInt(Get(root, "code"), -999);
            message = ToString(Get(root, "message"));
            data = Get(root, "data");
            return true;
        }

        public static object Get(IDictionary<string, object> dict, string key)
        {
            if (dict == null || key == null)
                return null;
            object value;
            return dict.TryGetValue(key, out value) ? value : null;
        }

        public static Dictionary<string, object> AsDict(object value)
        {
            if (value == null)
                return null;

            var dict = value as Dictionary<string, object>;
            if (dict != null)
                return dict;

            // Some serializers may produce non-generic IDictionary.
            var map = value as IDictionary;
            if (map == null)
                return null;

            var converted = new Dictionary<string, object>(StringComparer.Ordinal);
            foreach (DictionaryEntry entry in map)
            {
                if (entry.Key == null)
                    continue;
                converted[Convert.ToString(entry.Key, CultureInfo.InvariantCulture)] = entry.Value;
            }
            return converted;
        }

        public static ArrayList AsArray(object value)
        {
            if (value == null)
                return null;

            // JavaScriptSerializer.DeserializeObject returns JSON arrays as object[].
            var arrayList = value as ArrayList;
            if (arrayList != null)
                return arrayList;

            var objectArray = value as object[];
            if (objectArray != null)
                return new ArrayList(objectArray);

            var list = value as IList;
            if (list != null)
            {
                var copy = new ArrayList(list.Count);
                foreach (var item in list)
                    copy.Add(item);
                return copy;
            }

            return null;
        }

        public static string ToString(object value)
        {
            if (value == null || value is DBNull)
                return string.Empty;
            return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public static bool ToBool(object value, bool defaultValue = false)
        {
            if (value == null)
                return defaultValue;
            if (value is bool)
                return (bool)value;
            if (value is string)
            {
                bool b;
                if (bool.TryParse((string)value, out b))
                    return b;
            }
            try
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture) != 0;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static byte ToByteFlag(object value)
        {
            return (byte)(ToBool(value) ? 1 : 0);
        }

        public static int ToInt(object value, int defaultValue = 0)
        {
            if (value == null)
                return defaultValue;
            try
            {
                return Convert.ToInt32(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static uint ToUInt(object value, uint defaultValue = 0)
        {
            if (value == null)
                return defaultValue;
            try
            {
                return Convert.ToUInt32(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static ushort ToUShort(object value, ushort defaultValue = 0)
        {
            if (value == null)
                return defaultValue;
            try
            {
                return Convert.ToUInt16(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static float ToFloat(object value, float defaultValue = 0f)
        {
            if (value == null)
                return defaultValue;
            try
            {
                return Convert.ToSingle(value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return defaultValue;
            }
        }

        public static Dictionary<string, object> NewObject()
        {
            return new Dictionary<string, object>(StringComparer.Ordinal);
        }

        public static ArrayList NewArray()
        {
            return new ArrayList();
        }

        public static byte[] EncodeUtf8(string text)
        {
            return Encoding.UTF8.GetBytes(text ?? string.Empty);
        }
    }
}
