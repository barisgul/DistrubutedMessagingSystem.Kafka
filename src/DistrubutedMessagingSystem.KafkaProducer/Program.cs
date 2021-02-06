using DistrubutedMessagingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DistrubutedMessagingSystem.KafkaProducer
{
    public class Program
    {
        private static IConfigurationRoot configurationRoot;
        static async Task Main(string[] args)
        {
            configurationRoot = Configure();

            List<City> cities = configurationRoot.GetSection("Cities:City").Get<List<City>>();

            Producer producer = new Producer(configurationRoot);
            await producer.ProduceAsync(cities);
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
