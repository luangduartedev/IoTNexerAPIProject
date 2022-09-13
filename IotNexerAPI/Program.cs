using IoTNexerAPI.Models;
using IoTNexerAPI.Middlewares;
using IoTNexerAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISensorDataRepository<SensorData>, SensorDataRepository>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware(typeof(ErrorHandlingMiddleware));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
