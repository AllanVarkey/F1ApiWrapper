var builder = WebApplication.CreateBuilder(args);
var openF1BaseUrl = builder.Configuration["OpenF1BaseUrl"]
    ?? throw new InvalidOperationException("Configuration value 'OpenF1BaseUrl' is missing.");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpClient("OpenF1Api", client =>
{
    client.BaseAddress = new Uri(openF1BaseUrl, UriKind.Absolute);
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
