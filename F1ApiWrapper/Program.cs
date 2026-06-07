var builder = WebApplication.CreateBuilder(args);
var openF1BaseUrl = builder.Configuration["OpenF1BaseUrl"]
    ?? throw new InvalidOperationException("Configuration value 'OpenF1BaseUrl' is missing.");

// Add services to the container. this is a test comment
builder.Services.AddControllers();
builder.Services.AddHttpClient("OpenF1Api", client =>
{
    client.BaseAddress = new Uri(openF1BaseUrl, UriKind.Absolute);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "F1 API");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
