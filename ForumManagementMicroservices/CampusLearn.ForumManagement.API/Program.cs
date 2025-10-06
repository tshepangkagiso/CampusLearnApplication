var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
//configuring database
builder.Services.AddDbContext<ForumDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//configuring logging and logging to Seq
Log.Logger = new LoggerConfiguration() 
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url")??"")
    .CreateLogger();

//swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();

//rabbitmq configuration
builder.Services.AddScoped<IMessageStoreForum, MessageStoreForum>();
builder.Services.AddScoped<IForumMessagePublisher, ForumMessagePublisher>();
builder.Services.AddMassTransit(options =>
{
    options.AddConsumer<ForumMessageConsumer>();

    options.UsingRabbitMq((context, config) =>
    {
        var rabbitMqConnection = builder.Configuration["ConnectionStrings:RabbitMQ"];

        config.Host(new Uri(rabbitMqConnection), host =>{});

        config.ReceiveEndpoint("forum-queue", e =>
        {
            e.ConfigureConsumer<ForumMessageConsumer>(context);
            e.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(10)));
            e.BindDeadLetterQueue("forum-queue-error");
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
    # "https://localhost:6201;http://localhost:6200" - ports of this api 
    # "http://localhost:6200/swagger" - route to open swagger documentation
 */