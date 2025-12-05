using AccountsService.Infrastructure;
using AccountsService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var conn = builder.Configuration.GetConnectionString("AccountsDatabase")
           ?? Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__ACCOUNTS")
           ?? "Server=localhost,1433;Database=accounts;User Id=sa;Password=P@ssw0rd123!;TrustServerCertificate=True";

builder.Services.AddDbContext<AccountsDbContext>(options => options.UseSqlServer(conn));
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the background consumer
builder.Services.AddHostedService<PaymentEventsConsumer>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
