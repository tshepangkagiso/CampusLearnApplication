var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddHttpClient();
builder.Services.AddControllers();

builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("AgentSettings"));
builder.Services.AddScoped<IChatbotService, ChatbotService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Yarp", policy =>
    {
        policy.WithOrigins("http://campuslearn.yarpgateway.reverseproxy.api:8080", "http://localhost:7000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors("Yarp");
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
