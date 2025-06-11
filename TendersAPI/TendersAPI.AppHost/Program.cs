using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TendersAPI_WebApi>("tendersapi-service")
    .WithEnvironment("ApiSettings__ConcurrencyLimit", "20")
    .WithEnvironment("ApiSettings__BaseUrl", "https://tendersapi.example.com/api/tenders");

builder.Build().Run();
