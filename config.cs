using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace c_selfbot
{
    public static class config
    {
        public static string cfg = "default.json";

        public static settings load_config(string config_path) => 
            JsonConvert.DeserializeObject<settings>(File.ReadAllText(config_path));

        public static void save_config(string config_path, settings current) =>
            File.WriteAllText(config_path, JsonConvert.SerializeObject(current, Formatting.Indented));

    }
}
