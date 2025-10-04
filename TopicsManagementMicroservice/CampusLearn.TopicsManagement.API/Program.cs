var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//configuring database
builder.Services.AddDbContext<TopicsDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("TopicsDB")); //conection to db
});

//configuring logging and logging to Seq
Log.Logger = new LoggerConfiguration() 
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

//configuring swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

//rabbitmq configuration
builder.Services.AddScoped<ITopicMessagePublisher, TopicMessagePublisher>();
builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<TopicMessageConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        config.Host("localhost", "/", host =>
        {
            host.Username("myuser");
            host.Password("mypass");
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
    # "https://localhost:6101;http://localhost:6100" - ports of this api
    # "http://localhost:6100/swagger" - route to open swagger documentation
 */