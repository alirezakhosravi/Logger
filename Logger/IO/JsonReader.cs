using System.IO;
using Newtonsoft.Json;

namespace Raveshmand.Logger.IO
{
    public static class JsonReader
    {
        private static readonly object _lock = typeof(object);

        public static T Read<T>(string path, string fileName) where T : new()
        {
            lock (_lock)
            {
                T jsonObject = new T();
                if (!File.Exists(path))
                {
                    Write(jsonObject, path, fileName);
                }
                string file = $"{path}{fileName}";
                jsonObject = JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
                return jsonObject;
            }
        }

        public static void Write<T>(T jsonObject, string path, string fileName)
        {
            lock (_lock)
            {
                string output = JsonConvert.SerializeObject(jsonObject);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string file = $"{path}{fileName}";
                File.WriteAllText(file, output);
            }
        }
    }
}
