using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using My.Domain.Enums;
using My.Domain.Models;
using My.Kafka;
using Newtonsoft.Json;

namespace My.DispatchService
{
    public class DispatchService
    {

        public async Task Execute()
        {
            ConsumeResult<Null, string> subResult;
            DeliveryResult<Null, string> pubReult;
            string error;

            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();

            var producerReportTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Reported");

            var kafkaService = new KafkaService(configuration);

            while (true)
            {
                (subResult, error) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(error))
                {
                    var orderJsonData = subResult.Message.Value;
                    var order = JsonConvert.DeserializeObject<Order>(orderJsonData);

                    var report = DoDispatch(order!);
                    var reportJsonData = JsonConvert.SerializeObject(report);

                    (pubReult, error) = await kafkaService.Publish(producerReportTopicName, reportJsonData);


                }
            }
        }

        private Report DoDispatch(Order order)
        {
            Thread.Sleep(1);

            return new Report
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has been dispatched.",
                Status = Status.OrderDispatched,
                CreatedDate = DateTime.Now,
            };

        }
    }

    
}