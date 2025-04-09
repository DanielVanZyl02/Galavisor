using GalavisorApi.Middleware;
using GalavisorApi.Services;
using GalavisorApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddGoogleJwtAuthentication();
// builder.Services.AddDefaultAuthorization();
// builder.Services.AddHttpClient<AuthService>();
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddSingleton<ReviewService>();
builder.Services.AddSingleton<ReviewRepository>();

var app = builder.Build();

// app.UseHttpsRedirection();

// app.UseAuthorization();
app.MapControllers();

app.Run();