using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CQRS.Core.Consumers;
using DnsClient.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Post.Query.Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IServiceScope _scope;            // Keep the scope alive
        private Task _consumerTask;             // Background task
        private CancellationTokenSource _cts;   // Token to stop the consumer

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service running.");
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            // Create a DI scope that will live as long as the service lives
            _scope = _serviceProvider.CreateScope();

            var eventConsumer = _scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "SocialMediaPostEvent";

            // Start the Kafka consumer loop in background
            _consumerTask = Task.Run(() =>
            {
                try {
                    eventConsumer.Consume(topic); // Infinite loop inside Consume()
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Kafka consumer crashed.");
                }
            }, _cts.Token);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event consumer service stopped.");
            _cts.Cancel();

            if (_consumerTask != null)
            {
                await Task.WhenAny(_consumerTask, Task.Delay(3000, cancellationToken));
            }

            // Dispose the scope after the consumer stops
            _scope.Dispose();
            _cts.Dispose();

            _logger.LogInformation("Event consumer service stopped.");
        }
    }
}
