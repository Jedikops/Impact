var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TendersAPI_WebApi>("tendersapi-service");

builder.Build().Run();
