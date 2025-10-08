var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// Add health checks service
builder.Services.AddHealthChecks();

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

//configuring rabbitmq
builder.Services.AddSingleton<IMessageStoreTopic, MessageStoreTopic>();
builder.Services.AddSingleton<IMessageStoreForum, MessageStoreForum>();
builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<ForumMessageConsumer>();
    options.AddConsumer<TopicMessageConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        var rabbitMqConnection = builder.Configuration["ConnectionStrings:RabbitMQ"];

        config.Host(new Uri(rabbitMqConnection), host => { });

        config.ReceiveEndpoint("forum-queue", e =>
        {
            e.ConfigureConsumer<ForumMessageConsumer>(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            e.BindDeadLetterQueue("forum-queue-error");
        });

        config.ReceiveEndpoint("topic-queue", e =>
        {
            e.ConfigureConsumer<TopicMessageConsumer>(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            e.BindDeadLetterQueue("topic-queue-error");
        });
    });
});

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
    # "https://localhost:6301;http://localhost:6300" - ports of this api 
    # "http://localhost:6300/swagger" - route to open swagger documentation
 */