using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
    });

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
builder.Services.AddSingleton<RabbitMqPublisher>();



var app = builder.Build();
// Configure the HTTP request pipeline.


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
