using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPollCounter.Helpers
{
    public class Settings
    {
        public string AccessToken { get; set; }
        public string FilePath { get; set; }
        public string PostUrl { get; set; }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText("Settings.json", json);
        }

        public static Settings Load()
        {
            if (File.Exists("Settings.json"))
            {
                var json = File.ReadAllText("Settings.json");
                return JsonConvert.DeserializeObject<Settings>(json);
            }
            else
            {
                return new Settings();
            }
        }
    }
}
