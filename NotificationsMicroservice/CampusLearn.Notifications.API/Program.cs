using CampusLearn.Notifications.API.RabbitMQ;
using CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;
using CampusLearn.Notifications.API.Services.Email;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.


//configuring logging and logging to Seq
Log.Logger = new LoggerConfiguration() 
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url") ?? "")
    .CreateLogger();

//configuring swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();


builder.Services.AddControllers();

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SMTP"));
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddHostedService<TopicConsumerService>();
builder.Services.AddHostedService<ForumConsumerService>();

var app = builder.Build();
// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    //run swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
