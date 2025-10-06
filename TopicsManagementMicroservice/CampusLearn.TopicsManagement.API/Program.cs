var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
//configuring database
builder.Services.AddDbContext<TopicsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //conection to db
});

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

//rabbitmq configuration
builder.Services.AddScoped<IMessageStoreTopic, MessageStoreTopic>();
builder.Services.AddScoped<ITopicMessagePublisher, TopicMessagePublisher>();
builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<TopicMessageConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        var rabbitMqConnection = builder.Configuration["ConnectionStrings:RabbitMQ"];

        config.Host(new Uri(rabbitMqConnection), host => { });

        config.ReceiveEndpoint("topic-queue", e =>
        {
            e.ConfigureConsumer<TopicMessageConsumer>(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            e.BindDeadLetterQueue("topic-queue-error");
        });
    });
});



var app = builder.Build();
// Configure the HTTP request pipeline.

//run swagger in development mode
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
    # "https://localhost:6101;http://localhost:6100" - ports of this api
    # "http://localhost:6100/swagger" - route to open swagger documentation
 */