using GalavisorApi.Middleware;
using GalavisorApi.Services;
using GalavisorApi.Repositories;
using GalavisorApi.Data;
using GalavisorApi.Constants;


var builder = WebApplication.CreateBuilder(args);

var connectionString = ConfigStore.Get(ConfigKeys.DatabaseConnectionString);
builder.Services.AddSingleton(new DatabaseConnection(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddGoogleJwtAuthentication();
builder.Services.AddDefaultAuthorization();
builder.Services.AddHttpClient<AuthService>();

builder.Services.AddSingleton<ReviewService>();
builder.Services.AddSingleton<ReviewRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();