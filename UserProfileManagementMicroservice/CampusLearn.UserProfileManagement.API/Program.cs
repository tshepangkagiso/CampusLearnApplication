var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<UserManagementDbContext>(options =>
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();


//Dependency Injection
builder.Services.AddSingleton<IHashingService, HashingService>();
builder.Services.AddSingleton<IJwtService, JwtService>();
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

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
        var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

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



    // Auto-seed modules on startup
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedModulesAsync();
    }

    //auto-seed users
    using (var scope = app.Services.CreateScope())
    {
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await seeder.SeedUsersAsync();
    }
}


app.UseHttpsRedirection();

app.MapControllers();

app.Run();


/*
    # "https://localhost:6001;http://localhost:6000" - ports of this api
    # "http://localhost:6000/swagger" - route to open swagger documentation
 */