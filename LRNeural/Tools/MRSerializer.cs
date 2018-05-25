using MRNeural.Interface.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MRNeural.Tools
{
    public static class MRSerializer
    {
        public static async Task<bool> ToFile(string path, INeuralNet net, bool @override = true)
        {
            if (File.Exists(path))
            {
                if (@override)
                {
                    File.Delete(path);
                }
                else
                {
                    return false;
                }
            }

            var data = InnerToString(net);

            if (string.IsNullOrEmpty(data))
                return false;

            using (var ws = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                await ws.WriteAsync(bytes, 0, bytes.Length);
            }

            return true;
        }
        public static async Task<T> FromFile<T>(string path)
            where T : class, INeuralNet, new()
        {
            if (!File.Exists(path))
                return null;

            var bytes = await File.ReadAllBytesAsync(path);
            var data = Encoding.UTF8.GetString(bytes);

            if (string.IsNullOrWhiteSpace(data))
                return null;

            T result = null;

            try
            {
                result = (T)JsonConvert.DeserializeObject<T>(data, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })?.OnDeserialize();
            }
            catch
            {
                result = null;
            }

            return result;
        }

        public static byte[] ToBytes(object net)
        {
            if (net == null)
                return new byte[0];

            var data = InnerToString(net);

            if (string.IsNullOrWhiteSpace(data))
                return new byte[0];

            return Encoding.UTF8.GetBytes(data);
        }
        public static T FromBytes<T>(byte[] array)
            where T : class, INeuralNet, new()
        {
            if (array == null || array.Length == 0)
                return null;

            var data = Encoding.UTF8.GetString(array);
            if (string.IsNullOrEmpty(data))
                return null;

            return (T)JsonConvert.DeserializeObject<T>(data)?.OnDeserialize();
        }

        public static string ToString(object net) => InnerToString(net);
        public static T FromString<T>(string data)
            where T : class, INeuralNet, new()
        {
            T result = null;

            try
            {
                result = JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                result = null;
            }

            return result;
        }

        private static string InnerToString(object data)
        {
            if (data == null) return string.Empty;
            return JsonConvert.SerializeObject(data, Formatting.None, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
}
