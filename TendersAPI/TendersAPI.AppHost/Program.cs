using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TendersAPI_WebApi>("tendersapi-service")
    .WithEnvironment("ApiSettings__ConcurrencyLimit", "20");

builder.Build().Run();
