using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DistrubutedMessagingSystem.KafkaConsumer
{
    class Program
    {
        static IConfigurationRoot configurationRoot;
        static void Main(string[] args)
        {
            configurationRoot = Configure();
            Consumer consumer = new Consumer(configurationRoot);
            consumer.Consume();
        }

        private static IConfigurationRoot Configure()
        {
            // Build configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            return config;
        }
    }
}
