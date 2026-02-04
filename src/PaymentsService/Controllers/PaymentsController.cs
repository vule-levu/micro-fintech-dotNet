using Microsoft.AspNetCore.Mvc;
using PaymentsService.Infrastructure;
using PaymentsService.Domain.Entities;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using AccountsService.Grpc;


[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentsDbContext _db;

    //public PaymentsController(PaymentsDbContext db) => _db = db;

    private readonly AccountsGrpc.AccountsGrpcClient _accountsClient;

    public PaymentsController(
        PaymentsDbContext db,
        AccountsGrpc.AccountsGrpcClient accountsClient)
    {
        _db = db;
        _accountsClient = accountsClient;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
    {
        // 1️.Validate account via gRPC
        var accountResponse = await _accountsClient.GetAccountAsync(
            new GetAccountRequest
            {
                AccountId = dto.AccountId.ToString()
            });

        if (!accountResponse.Exists)
        {
            return BadRequest("Account does not exist");
        }

        // 2️.Continue normal payment flow
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            AccountId = dto.AccountId,
            Amount = dto.Amount,
            Currency = dto.Currency ?? "USD",
            Status = "PENDING",
            CreatedAt = DateTime.UtcNow
        };

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();

        try
        {
            PublishPaymentCreatedEvent(payment);
        }
        catch
        {
            // swallow for demo
        }

        return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var p = await _db.Payments.FindAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    private void PublishPaymentCreatedEvent(Payment payment)
    {
        // Read RabbitMQ URI from environment; default to localhost for dev
        var rabbitUri = Environment.GetEnvironmentVariable("RABBITMQ_URI")
                        ?? "amqp://guest:guest@localhost:5672";

        var factory = new ConnectionFactory
        {
            Uri = new Uri(rabbitUri)
        };

        // For a demo it's OK to create a connection/channel per publish.
        // In production make the connection/channel long-lived and injected via DI.
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        // Create a durable fanout exchange called "payments"
        channel.ExchangeDeclare(exchange: "payments", type: ExchangeType.Fanout, durable: true);

        var payload = new
        {
            paymentId = payment.Id,
            accountId = payment.AccountId,
            amount = payment.Amount,
            currency = payment.Currency,
            status = payment.Status,
            createdAt = payment.CreatedAt
        };

        var json = JsonSerializer.Serialize(payload);
        var body = Encoding.UTF8.GetBytes(json);

        var props = channel.CreateBasicProperties();
        props.DeliveryMode = 2; // persistent

        channel.BasicPublish(exchange: "payments", routingKey: "", basicProperties: props, body: body);
    }
}

// DTO (feel free to move to DTOs/CreatePaymentDto.cs)
public class CreatePaymentDto
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
}
