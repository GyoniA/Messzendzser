using Newtonsoft.Json;

namespace Messzendzser.Utils
{
    public class Configuration
    {
        public static Configuration Load(string configFileName)
        {
                string configPath = Path.Combine(Directory.GetCurrentDirectory(), configFileName);
                if (!File.Exists(configPath))
                {
                    Console.WriteLine("Configuration file was not found");
                    System.Environment.Exit(-1);
                }
                string text = File.ReadAllText(configPath);
                if (string.IsNullOrEmpty(text))
                {
                    Console.WriteLine("Configuration file is empty");
                    System.Environment.Exit(-1);
                }
                Configuration settings = JsonConvert.DeserializeObject<Configuration>(text);
                if (settings == null)
                {
                    Console.WriteLine("Invalid configuration file");
                    System.Environment.Exit(-1);
                }
                return settings;
        }
        public string DbConnectionString { get; set; } = null!;
    }
}
