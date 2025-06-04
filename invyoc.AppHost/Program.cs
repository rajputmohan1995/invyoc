var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.invyoc>("invyoc");

builder.Build().Run();
