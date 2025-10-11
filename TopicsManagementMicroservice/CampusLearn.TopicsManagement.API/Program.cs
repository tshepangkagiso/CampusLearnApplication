using Minio;

var builder = WebApplication.CreateBuilder(args);
// Add health checks service
builder.Services.AddHealthChecks();

builder.Services.AddControllers();
// Add services to the container.
//configuring database
builder.Services.AddDbContext<TopicsDbContext>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("LocalTestConnection")); // for local ssms
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //connecting to docker db
});

//configuring logging and logging to Seq
Log.Logger = new LoggerConfiguration() 
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url") ?? "")
    .CreateLogger();

// Configure MinIO
builder.Services.AddMinio(configureClient =>
{
    configureClient
        .WithEndpoint("minio", 9000)
        .WithCredentials(builder.Configuration["MinIO:AccessKey"], builder.Configuration["MinIO:SecretKey"])
        .WithSSL(false)
        .Build();
});

builder.Services.AddScoped<MinioService>();

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

app.MapHealthChecks("/health");

//run swagger in development mode
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}

//auto migration
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TopicsDbContext>();

        try
        {
            // First, ensure the database exists
            if (!dbContext.Database.CanConnect())
            {
                Log.Information("Database doesn't exist. Creating...");

                // Create the database if it doesn't exist
                dbContext.Database.EnsureCreated();
                Log.Information("Database created successfully.");
            }

            // Then apply migrations
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Log.Information($"Applying {pendingMigrations.Count()} pending migrations...");
                dbContext.Database.Migrate();
                Log.Information("Migrations applied successfully.");
            }
            else
            {
                Log.Information("No pending migrations.");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Database initialization error: {ex.Message}");
        }
    }
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

/*
    # "https://localhost:6101;http://localhost:6100" - ports of this api
    # "http://localhost:6100/swagger" - route to open swagger documentation
 */