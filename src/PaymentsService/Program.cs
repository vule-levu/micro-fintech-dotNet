
using Microsoft.EntityFrameworkCore;
using PaymentsService.Infrastructure;

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

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapControllers();
app.Run();
