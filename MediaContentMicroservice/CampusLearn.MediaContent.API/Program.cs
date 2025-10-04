using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

Log.Logger = new LoggerConfiguration() //configuring logging and logging to Seq
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Host.UseSerilog();
builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    //run swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
    # "https://localhost:6401;http://localhost:6400" - ports of this api
    # "http://localhost:6400/swagger" - route to open swagger documentation
 */