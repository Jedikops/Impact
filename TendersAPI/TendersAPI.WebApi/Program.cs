using Serilog;
using TendersApi.WebApi;
using TendersAPI.WebApi.Filters;
using TendersAPI.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();

builder.RegisterServices();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddControllers((options) =>
{
    options.Filters.Add<LogBadRequestAndStatusCodeFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<BadRequestLoggingMiddleware>();

app.Run();

