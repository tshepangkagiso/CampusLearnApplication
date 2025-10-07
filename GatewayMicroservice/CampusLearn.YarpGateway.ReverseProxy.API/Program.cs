var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
    options.Limits.MaxRequestBodySize = null; // no artificial limit
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information() // lighter logging level
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url") ?? "")
    .CreateLogger();
builder.Host.UseSerilog();

// Add HttpClient for fetching Swagger JSON from microservices
builder.Services.AddHttpClient();

// Configure YARP
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Only HTTP, skip HTTPS redirect (since Docker is internal HTTP only)
app.UseCors("ClientApp");
app.MapReverseProxy();

app.Run();


