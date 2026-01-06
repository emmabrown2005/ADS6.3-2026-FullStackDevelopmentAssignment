using MapDistanceAPI.MiddleWare;
using MapDistanceAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// State + logic services
builder.Services.AddSingleton<MapStore>();
builder.Services.AddSingleton<PathService>();

var app = builder.Build();

app.UseHttpsRedirection();

// API key middleware must run before MapController endpoints
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
