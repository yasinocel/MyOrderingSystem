using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using My.Domain.Models;
using My.Kafka;
using Newtonsoft.Json;

namespace My.ReportingService
{
    public class ReportingService
    {

        public Task Execute(Action<Report> writeConsole)
        {
            ConsumeResult<Null, string>? subResult = null;
            string error;

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true).Build();


            var kafkaService = new KafkaService(configuration);

            while (true)
            {
                (subResult, error) = kafkaService.Subscribe();
                if (string.IsNullOrEmpty(error))
                {
                    var report = JsonConvert.DeserializeObject<Report>(subResult.Message.Value);
                    writeConsole?.Invoke(report!);
                }

            }
        }


    }
}