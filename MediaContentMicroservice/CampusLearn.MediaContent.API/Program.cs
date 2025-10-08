var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

// Add health checks service
builder.Services.AddHealthChecks();


Log.Logger = new LoggerConfiguration() //configuring logging and logging to Seq
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url") ?? "")
    .CreateLogger();

//Add MinioService as a Singleton Service
builder.Services.AddSingleton<MinioService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();
builder.Services.AddControllers();


var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapHealthChecks("/health");
if (app.Environment.IsDevelopment())
{
    //run swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/*
    # "https://localhost:6401;http://localhost:6400" - ports of this api
    # "http://localhost:6400/swagger" - route to open swagger documentation
 */