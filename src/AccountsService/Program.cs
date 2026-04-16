using AccountsService.Infrastructure;
using AccountsService.Services;
using Microsoft.EntityFrameworkCore;
using Grpc.Core;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("AccountsDatabase")
           ?? Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__ACCOUNTS")
           ?? "Server=localhost,1433;Database=accounts;User Id=sa;Password=P@ssw0rd123!;TrustServerCertificate=True";

builder.Services.AddDbContext<AccountsDbContext>(options => options.UseSqlServer(conn));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC endpoint (HTTP/2)
    options.ListenLocalhost(6001, o =>
    {
        o.Protocols = HttpProtocols.Http2;
    });

    // REST endpoint (HTTP/1.1)
    options.ListenLocalhost(6002, o =>
    {
        o.Protocols = HttpProtocols.Http1;
    });
});

builder.Services.AddGrpc();
// Register the background consumer
builder.Services.AddHostedService<PaymentEventsConsumer>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});



var app = builder.Build();
app.UseCors("AllowAll");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGrpcService<AccountsGrpcService>();
app.MapGet("/", () => "AccountsService gRPC endpoint");
app.MapControllers();
app.Run();
