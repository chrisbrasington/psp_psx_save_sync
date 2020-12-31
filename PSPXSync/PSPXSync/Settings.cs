using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSPXSync
{
    public static class Settings
    {
        public static Dictionary<string, object> Configuration = new Dictionary<string, object>();

        public static bool Contains(string key)
        {
            return Configuration.ContainsKey(key);
        }

        public static void Set(string key, string value)
        {
            Configuration[key] = value;
        }

        public static void Set(string key, object value)
        {
            Configuration[key] = value;
        }


        public static object Get(string key)
        {
            if (Configuration.ContainsKey(key))
            {
                return Configuration[key];
            }
            return "";
        }

        public static void Load()
        {
            Configuration = ReadFromJsonFile<Dictionary<string, object>>("settings.json");

            if(Configuration == null)
            {
                Configuration = new Dictionary<string, object>();
            }
        }

        public static void Save()
        {
            using (StreamWriter writer = File.CreateText("settings.json"))
            {
                writer.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(Configuration));
            }
        }

        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            if (File.Exists(filePath))
            {
                TextReader reader = null;
                try
                {
                    reader = new StreamReader(filePath);
                    var fileContents = reader.ReadToEnd();
                    return JsonConvert.DeserializeObject<T>(fileContents);
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }

            return JsonConvert.DeserializeObject<T>("{}");
        }

    }
}
