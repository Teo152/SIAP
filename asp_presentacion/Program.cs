using asp_presentacion;
using asp_presentacion.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

// Necesario para mapear usuarios a conexiones
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddHttpContextAccessor();

// Vincular Startup
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Ejecutar pipeline
startup.Configure(app, app.Environment);
app.MapHub<ChatHub>("/chathub");

app.Run();