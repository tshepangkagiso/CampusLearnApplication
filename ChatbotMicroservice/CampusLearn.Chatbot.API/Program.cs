using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<AgentSettings>(builder.Configuration.GetSection("AgentSettings"));
builder.Services.AddScoped<IChatbotService, ChatbotService>();

var allowedOrigins = new[]
{
	"http://campuslearn.yarpgateway.reverseproxy.api:8080",
	"http://localhost:7000",
    "http://localhost:8080"    
};

builder.Services.AddCors(options =>
{
	options.AddPolicy("Yarp", policy =>
	{
		policy.WithOrigins(allowedOrigins)
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

builder.Services.Configure<ForwardedHeadersOptions>(opts =>
{
	opts.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;

});

var app = builder.Build();

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("Yarp");

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
