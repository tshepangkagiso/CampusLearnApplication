var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("AgentSettings"));
builder.Services.AddScoped<IChatbotService, ChatbotService>();

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
app.UseCors("Yarp");
app.UseHttpsRedirection();


if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
