using IoTNexerAPI.Domain.Entity;
using IoTNexerAPI.Domain.Repository;
using IoTNexerAPI.Domain.Interfaces;
using IoTNexerAPI.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISensorDataRepository<SensorData>, SensorDataRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
