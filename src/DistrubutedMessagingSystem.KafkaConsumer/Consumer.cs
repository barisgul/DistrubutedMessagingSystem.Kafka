
using Confluent.Kafka;
using DistrubutedMessagingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DistrubutedMessagingSystem.KafkaConsumer
{
    public class Consumer
    {
        IConfigurationRoot configurationRoot;
        WeatherService weatherService;
        public Consumer(IConfigurationRoot configurationRoot)
        {
            this.configurationRoot = configurationRoot;
            weatherService = new WeatherService(configurationRoot);
        }
        public void Consume()
        {
            string serverAddress = configurationRoot.GetSection("Kafka:ServerAddress").Get<string>();
            string topic = configurationRoot.GetSection("Kafka:Topic").Get<string>();
            string port = configurationRoot.GetSection("Kafka:Port").Get<string>();

            var config = new ConsumerConfig
            {
                BootstrapServers = serverAddress + ":" + port.Trim(),
                AutoOffsetReset = AutoOffsetReset.Earliest,
                GroupId = "my-consumer-group"
            };

            using (var c = new ConsumerBuilder<Null, string>(config).Build())
            {
                c.Subscribe(topic);

                CancellationTokenSource cts = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true;
                    cts.Cancel();
                };

                try
                {
                    while (true)
                    {
                        try
                        {
                            var cr = c.Consume(cts.Token);
                            City city = JsonConvert.DeserializeObject<City>(cr.Value);

                            string deliveryMessage = string.Format("Delivery succeeded for City is {0} at {1}.", city.Name, cr.TopicPartitionOffset);
                            
                            //make a request to OpenWeatherApi for getting weather conditions
                            WeatherInfo weatherInfo = weatherService.GetWeatherInfo(city);

                            string weatherMessage = $"The weather is {weatherInfo.weather[0].description.ToUpper()} at {city.Name.ToUpper()}";
                            Console.WriteLine(deliveryMessage+" "+weatherMessage); 
                        }
                        catch (ConsumeException ex)
                        {
                            Console.WriteLine($"Error occured: {ex.Error.Reason}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error occured: {ex.ToString()}");
                        }
                    }
                    c.Commit();

                }
                catch(OperationCanceledException ex)
                {
                    Console.WriteLine("Error: ",ex.Message);
                }
            }
        }
    }
}

