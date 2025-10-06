var builder = WebApplication.CreateBuilder(args);

// Configuring logging and logging to Seq
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(builder.Configuration.GetValue<string>("Seq:Url") ?? "")
    .CreateLogger();
builder.Host.UseSerilog();

// Add HttpClient for fetching Swagger JSON from microservices
builder.Services.AddHttpClient();

// Configuring the YARP reverse proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//configuring cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp",policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();
app.UseCors("ClientApp");
app.UseHttpsRedirection();
app.MapReverseProxy(); // Handles the mapping of our reverse proxy

app.Run();

/*
    # "https://localhost:6601;http://localhost:6600" - ports of this api
    # "http://localhost:6600/swagger" - route to open swagger documentation
 */

/*
 * Run following docker commands on cmd:
 
    1) userprofiledb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1435:1433 --name sqlserver.userprofile -d mcr.microsoft.com/mssql/server:2025-latest

    2) topicsdb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1436:1433 --name sqlserver.topics -d mcr.microsoft.com/mssql/server:2025-latest

    3) forumdb:
    docker run --restart unless-stopped -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=Password123!" -p 1438:1433 --name sqlserver.forum -d mcr.microsoft.com/mssql/server:2025-latest

    4) seq:
    docker run -d --name seq --restart unless-stopped -e ACCEPT_EULA=Y -e SEQ_PASSWORD=admin -p 5341:80 datalust/seq:latest

    5) minio:
    docker run --restart unless-stopped -d --name minio -p 9000:9000 -p 9001:9001 -e MINIO_ROOT_USER=minioadmin -e MINIO_ROOT_PASSWORD=minioadmin123 minio/minio:latest server /data --console-address ":9001"

    6) rabbitmq:
    docker run -d --name rabbitmq --restart unless-stopped -p 5672:5672 -p 15672:15672 -e RABBITMQ_DEFAULT_USER=guest -e RABBITMQ_DEFAULT_PASS=guest rabbitmq:3-management
 */


/*
 Environment Variables for Each Service 

   User Profile Service 
   - ConnectionStrings__DefaultConnection=Server=sqlserver.userprofile;Database=UserProfileDb;User Id=sa;Password=Password123!;TrustServerCertificate=true;

   Topics Management Service
   - ConnectionStrings__DefaultConnection=Server=sqlserver.topics;Database=TopicsDb;User Id=sa;Password=Password123!;TrustServerCertificate=true;

   Forum Management Service 
   - ConnectionStrings__DefaultConnection=Server=sqlserver.forum;Database=ForumDb;User Id=sa;Password=Password123!;TrustServerCertificate=true;


 Shared for ALL Services 
     - ConnectionStrings__RabbitMQ=amqp://guest:guest@rabbitmq:5672
     - Seq__Url=http://seq:5341
     - MinIO__Endpoint=http://minio:9000
     - MinIO__AccessKey=minioadmin
     - MinIO__SecretKey=minioadmin123
 */
/*
Configuration to backend services - this are routes client must call
    # http://localhost:6000/users - maps to user profile management api
    # http://localhost:6100/topic - maps to topics management api
    # http://localhost:6200/forum - maps to forum management api
    # http://localhost:6300/notifications - maps to notifications api
    # http://localhost:6400/media - maps to media content api
    # http://localhost:6500/messages - maps to private messaging api
*/