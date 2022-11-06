using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using My.AppSettings;
using System.Xml.XPath;

namespace My.Kafka
{
    public class KafkaService
    {

        private IProducer<Null, string>? _producerBuilder;
        private IConsumer<Null, string>? _consumerBuilder;
        private CancellationTokenSource? _cancellationTokenSource;

        public KafkaService(IConfiguration configuration)
        {
            var producerSettings = AppSettings.AppSettings.GetConfig(configuration, "Producer");
            var consumerSettings = AppSettings.AppSettings.GetConfig(configuration, "Consumer");

            if (producerSettings != null && producerSettings.Count > 0)
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = producerSettings?.Where(x => x.Key.Equals("BootstrapServers")).FirstOrDefault().Value
                };

                _producerBuilder = new ProducerBuilder<Null, string>(producerConfig).Build();
            }

            if (consumerSettings != null && consumerSettings.Count > 0)
            {
                var consumerConfig = new ConsumerConfig()
                {
                    BootstrapServers = consumerSettings?.Where(x => x.Key.Equals("BootstrapServers")).FirstOrDefault().Value,
                    GroupId = consumerSettings?.Where(x => x.Key.Equals("GroupId")).FirstOrDefault().Value,
                    AutoOffsetReset = AutoOffsetReset.Earliest
                };

                _consumerBuilder = new ConsumerBuilder<Null, string>(consumerConfig).Build();
                _consumerBuilder.Subscribe(consumerSettings?.Where(x => x.Key.Equals("TopicName")).FirstOrDefault().Value);


                _cancellationTokenSource = new CancellationTokenSource();
                Console.CancelKeyPress += (_, e) =>
                {
                    e.Cancel = true; //prevent the process from terminating.
                    _cancellationTokenSource.Cancel();
                };
            }



        }

        public async Task<(DeliveryResult<Null, string>, string)> Publish(String topicName, string data)
        {
            DeliveryResult<Null, string>? result = null;
            try
            {
                result = await _producerBuilder!.ProduceAsync(topicName, new Message<Null, string> { Value = data });
                return (result, string.Empty);
            }
            catch (ProduceException<Null, string> e)
            {
                return (result!, e.Error.Reason);
            }
        }

        public (ConsumeResult<Null, string>, string) Subscribe()
        {
            ConsumeResult<Null, string>? result = null;
            try
            {
                result = _consumerBuilder!.Consume(_cancellationTokenSource!.Token);
                return (result, String.Empty);
            }
            catch (ConsumeException e)
            {
                return (result!, e.Error.Reason);
            }
        }



    }
}