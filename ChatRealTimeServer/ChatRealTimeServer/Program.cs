using ChatRealTimeServer.Config;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapHub<Hub.Handle>("/chat");
app.Run();
