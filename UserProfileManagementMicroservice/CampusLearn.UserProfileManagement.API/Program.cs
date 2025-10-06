var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<UserManagementDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")); //connecting to db
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
    # "https://localhost:6001;http://localhost:6000" - ports of this api
    # "http://localhost:6000/swagger" - route to open swagger documentation

    Configuration to backend services - this are routes client must call
    # http://localhost:6000/users - maps to user profile management api
    # http://localhost:6100/topic - maps to topics management api
    # http://localhost:6200/forum - maps to forum management api
    # http://localhost:6300/notifications - maps to notifications api
    # http://localhost:6400/media - maps to media content api
    # http://localhost:6500/messages - maps to private messaging api

 */