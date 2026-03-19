using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PlaywrightTests.Config
{
    public class FrameworkConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Browser { get; set; } = "chromium";
        public bool Headless { get; set; } = true;
        public int Timeout { get; set; } = 30000;
    }

    public static class ConfigManager
    {
        public static FrameworkConfig Settings { get; }

        static ConfigManager()
        {
            string environment = Environment.GetEnvironmentVariable("TEST_ENV") ?? "dev";

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"Config/Environments/appsettings.{environment}.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            
            Settings = new FrameworkConfig();
            configuration.Bind(Settings);
        }
    }
}