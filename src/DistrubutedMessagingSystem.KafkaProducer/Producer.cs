using Confluent.Kafka;
using DistrubutedMessagingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistrubutedMessagingSystem.KafkaProducer
{
    public class Producer : IProducer
    {
        readonly IConfigurationRoot configurationRoot;
        public Producer(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
        }

        public async Task ProduceAsync(List<City> cities)
        {
            var tasks = new List<Task<bool>>();
            foreach (var item in cities)
            {
                tasks.Add(SendMessageAsync(item));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// Send messages to kafka asynchronous
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        private async Task<bool> SendMessageAsync(City city)
        {
            string serverAddress = configurationRoot.GetSection("Kafka:ServerAddress").Get<string>();
            string topic = configurationRoot.GetSection("Kafka:Topic").Get<string>();
            string port = configurationRoot.GetSection("Kafka:Port").Get<string>();

            var config = new ProducerConfig
            {
                BootstrapServers = serverAddress + ":" + port.Trim()
            };

            //Serialize City object and send as log  message to kafka topic
            string cityAsJson = JsonConvert.SerializeObject(city);

            try
            {
                // Create a producer that can be used to send messages to kafka that have no key and a value of type string 
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    // Send the message to our test topic in Kafka  
                    var dr = await producer.ProduceAsync(topic, new Message<Null, string> { Value = cityAsJson });
                    Console.WriteLine($"Produced message '{dr.Value}' to topic {dr.Topic}, partition {dr.Partition}, offset {dr.Offset}");

                    return true;
                } 
            }
            catch (ProduceException<string,City> ex)
            {
                Console.WriteLine($"Delivery failed: {ex.Error.Reason}");
                return false;
            }
        }
    }
}
