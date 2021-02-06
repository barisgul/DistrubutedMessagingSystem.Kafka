using DistrubutedMessagingSystem.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistrubutedMessagingSystem.KafkaProducer
{
    public interface IProducer
    {
        Task ProduceAsync(List<City> cities);
    }
}