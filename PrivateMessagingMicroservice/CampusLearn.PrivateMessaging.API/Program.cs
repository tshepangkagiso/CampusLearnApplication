using CampusLearn.PrivateMessaging.API.Database;
using CampusLearn.PrivateMessaging.API.RabbitMQ;
using CampusLearn.PrivateMessaging.API.Signal_R.ChatRoomService;
using CampusLearn.PrivateMessaging.API.Signal_R.Hubs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

//configuring database
builder.Services.AddDbContext<PrivateMessagesDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); 
});

//configuring signal r
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();
builder.Services.AddSignalR();

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
builder.Services.AddControllers();

builder.Services.AddHostedService<TopicConsumerService>();


var app = builder.Build();
// Configure the HTTP request pipeline.

app.MapHub<ChatHub>("/chatHub");

if (app.Environment.IsDevelopment())
{
    //run swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

//auto migration
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<PrivateMessagesDbContext>();

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
