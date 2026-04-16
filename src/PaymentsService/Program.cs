
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure;
using Grpc.Net.Client;

using AccountsService.Grpc;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext - read connection string from environment or appsettings
var conn = builder.Configuration.GetConnectionString("PaymentsDatabase")
           ?? Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULT")
           ?? "Host=localhost;Port=5432;Database=payments;Username=postgres;Password=postgres";

builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(conn));

// add controllers etc.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddGrpcClient<AccountsGrpc.AccountsGrpcClient>(o =>
{
    o.Address = new Uri("http://localhost:6001"); // MUST be the gRPC port
});
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
app.MapControllers();
app.Run();
