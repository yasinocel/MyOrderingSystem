using Microsoft.Extensions.Configuration;
using My.Domain.Enums;
using My.Domain.Models;
using My.Kafka;
using Newtonsoft.Json;

namespace My.OrderService
{
    public class OrderService
    {

        public async Task Execute()
        {
            var configuration = new ConfigurationBuilder()
         .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();

            var producerSubmittedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Submitted");
            var producerReportedTopicName = AppSettings.AppSettings.GetTopicName(configuration, "Reported");

            var kafkaService = new KafkaService(configuration);

            while (true)
            {
                var (order, report) = DoOrdering();

                var reportJsonData = JsonConvert.SerializeObject(report);
                var (result, error) = await kafkaService.Publish(producerReportedTopicName, reportJsonData);

                var orderJsonData = JsonConvert.SerializeObject(order);
                (result, error) = await kafkaService.Publish(producerSubmittedTopicName, orderJsonData);
            }
        }


        private (Order, Report) DoOrdering()
        {
            var rnd = new Random();

            Thread.Sleep(10000);

            var pId = rnd.Next(111111, 999999);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                ProductId = pId,
                ProductName = $"Product {pId}",
                Quantity = rnd.Next(1, 10),
                Price = rnd.Next(1, 100),
                CreatedDate = DateTime.Now,
            };


            var report = new Report
            {
                Id = Guid.NewGuid(),
                Order = order,
                Details = "Order has been submitted.",
                Status = Status.OrderSubmitted,
                CreatedDate = DateTime.Now,
            };


            return (order, report);
        }


    }


}