using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using My.Domain.Enums;
using My.Domain.Models;
using My.Kafka;
using Newtonsoft.Json;

namespace My.InventoryService
{
    public class InventoryService
    {

        public async Task Execute()
        {

            ConsumeResult<Null, string> subResult;
            DeliveryResult<Null, string> pubResult;
            string error;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();

            var producerValidatedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Validated");
            var producerReportedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Reported");

            var kafkaService = new KafkaService(configuration);

            while (true)
            {
                (subResult, error) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(error))
                {
                    var orderJsonData = subResult.Message.Value;
                    var order = JsonConvert.DeserializeObject<Order>(orderJsonData);

                    var (report, isvalidated) = DoInventory(order!);

                    string reportJsonData = JsonConvert.SerializeObject(report);
                    (pubResult, error) = await kafkaService.Publish(producerReportedTopicName, reportJsonData);

                    if (isvalidated)
                    {
                        (pubResult, error) = await kafkaService.Publish(producerValidatedTopicName, orderJsonData);
                    }
                }

            }
        }

        private (Report,bool) DoInventory(Order order)
        {
            bool isValidated = false;

            Thread.Sleep(1);

            var report = new Report
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has not been validated due to out of stock.",
                Status = Status.OrderOutOfStock,
                CreatedDate = DateTime.Now,
            };

            if (order.Quantity < 7)
            {
                report.Details = "Order has been validated.";
                report.Status = Status.OrderValidated;
                isValidated = true;
            }

            return (report, isValidated);
        }
    }
}