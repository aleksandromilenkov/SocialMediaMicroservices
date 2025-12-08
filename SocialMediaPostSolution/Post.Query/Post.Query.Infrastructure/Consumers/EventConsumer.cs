using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IEventHandler _eventHandler;
        private readonly ILogger<EventConsumer> _logger;
        public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler, ILogger<EventConsumer> logger)
        {
            _config = config.Value;
            _eventHandler = eventHandler;
            _logger = logger;
        }
        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config).SetKeyDeserializer(Deserializers.Utf8).SetValueDeserializer(Deserializers.Utf8).Build();
            _logger.LogInformation("Subscribing to Kafka topic: {Topic}", topic);
            consumer.Subscribe(topic);
            while (true)
            {
                var consumeResult = consumer.Consume();
                if (consumeResult?.Message == null) continue;
                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                var handlerMethod = _eventHandler.GetType().GetMethod("On", new Type[] { @event.GetType() });
                if (handlerMethod == null) throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method");
                handlerMethod.Invoke(_eventHandler, new object[] { @event });
                consumer.Commit(consumeResult);
            }
        }
    }
}
