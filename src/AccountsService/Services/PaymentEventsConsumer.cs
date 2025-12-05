using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using AccountsService.Infrastructure;
using AccountsService.Domain.Entities;

namespace AccountsService.Services
{
    // Background worker that consumes PaymentCreated events and updates accounts.
    public class PaymentEventsConsumer : BackgroundService
    {
        private IConnection? _connection;
        private IModel? _channel;
        private readonly IServiceProvider _services;
        private const string ExchangeName = "payments";
        private const string QueueName = "accounts.payments.queue";

        public PaymentEventsConsumer(IServiceProvider services)
        {
            _services = services;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            var rabbitUri = Environment.GetEnvironmentVariable("RABBITMQ_URI") ?? "amqp://guest:guest@localhost:5672";
            var factory = new ConnectionFactory { Uri = new Uri(rabbitUri) };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout, durable: true);
            _channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
            _channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: "");

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_channel == null)
                throw new InvalidOperationException("RabbitMQ channel is not initialized.");

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var payload = JsonSerializer.Deserialize<PaymentCreatedPayload>(json);

                    if (payload is not null)
                    {
                        using var scope = _services.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AccountsDbContext>();

                        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == payload.accountId);
                        if (account == null)
                        {
                            account = new Account
                            {
                                Id = payload.accountId,
                                Owner = "Unknown",
                                Balance = 0m,
                                CreatedAt = DateTime.UtcNow
                            };
                            db.Accounts.Add(account);
                        }

                        // DEMO: apply payment as debit (subtract)
                        account.Balance -= payload.amount;

                        await db.SaveChangesAsync();
                    }

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception)
                {
                    // demo: nack without requeue to avoid loops. In real app implement retries/DLQ.
                    if (_channel != null)
                        _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
            };

            _channel.BasicConsume(queue: QueueName, autoAck: false, consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
            }
            catch { /* ignore dispose errors in demo */ }

            base.Dispose();
        }

        private record PaymentCreatedPayload(Guid paymentId, Guid accountId, decimal amount, string currency, string status, DateTime createdAt);
    }
}
