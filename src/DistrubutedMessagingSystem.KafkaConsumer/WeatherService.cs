using DistrubutedMessagingSystem.Domain.Entities;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System;

namespace DistrubutedMessagingSystem.KafkaConsumer
{
    public class WeatherService
    {
        private IRestClient restClient;
        readonly IConfiguration configuration;

        public WeatherService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public WeatherInfo GetWeatherInfo(City city)
        {
            if (city == null)
                throw new ArgumentNullException(nameof(city));

            string baseUrl = configuration.GetSection("OpenWeatherApi:BaseUrl").Get<string>();
            restClient = new RestClient(baseUrl);

            string apiKey = configuration.GetSection("OpenWeatherApi:ApiKey").Get<string>();

            //pass cityname and api key to request endpoint
            var request = new RestRequest($"/data/2.5/weather?q={city.Name}&appid={apiKey}", Method.GET);

            var queryResult = restClient.Execute<WeatherInfo>(request).Data; //execute request
            
            return queryResult;
        }
    }
}
