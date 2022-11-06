using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using My.Domain.Enums;
using My.Domain.Models;
using My.Kafka;
using Newtonsoft.Json;

namespace My.PaymentService
{
    public class PaymentSerivce
    {

        public async Task Execute()
        {

            ConsumeResult<Null, string> subResult;
            DeliveryResult<Null, string> pubResult;
            string error;

            var configuration = new ConfigurationBuilder()
             .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();

            var producerProcessedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Processed");
            var producerReportedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Reported");

            var kafkaService = new KafkaService(configuration);

            while (true)
            {
                (subResult, error) = kafkaService.Subscribe();

                if (string.IsNullOrEmpty(error))
                {
                    var orderJsonData = subResult.Message.Value;
                    var order = JsonConvert.DeserializeObject<Order>(orderJsonData);

                    var (report, isProcessed) = DoPayment(order!);

                    var reportJsonData = JsonConvert.SerializeObject(report);
                    (pubResult, error) = await kafkaService.Publish(producerReportedTopicName, reportJsonData);

                    if (isProcessed)
                    {
                        (pubResult, error) = await kafkaService.Publish(producerProcessedTopicName, orderJsonData);
                    }
                }
            }
        }

        private (Report, bool) DoPayment(Order order)
        {
            var isProcessed = false;

            Thread.Sleep(1);

            var report = new Report
            {
                Id = Guid.NewGuid(),
                Order = order,
                CreatedDate = DateTime.Now,
            };

            if (order.Price > 50)
            {
                report.Details = "Order has not been processed due to failed payment.";
                report.Status = Status.PaymentFailed;
            }
            else
            {
                report.Details = "Order has been processed.";
                report.Status = Status.PaymentProcessed;
                isProcessed = true;
            }


            return (report, isProcessed);

        }
    }
}