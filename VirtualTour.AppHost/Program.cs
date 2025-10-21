var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.fKVPortalAspireWeb_ApiService>("apiservice");

builder.AddProject<Projects.fKVPortalAspireWeb_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
