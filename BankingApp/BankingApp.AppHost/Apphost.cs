using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var pass = builder.AddParameter("Password");

var sql = builder.AddSqlServer("SqlServer", pass)
    .WithHostPort(9433)
    .WithImageTag("2022-latest");

var appDb = sql
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase("BankingDb");

var api = builder.AddProject<BankingApp_Api>("Api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithEnvironment("Console__LogLevel__Default", "Information")
    .WithEnvironment("Console__LogLevel__Microsoft", "Information")
    .WithEnvironment("Logging__LogLevel__Default", "Information")
    .WithEnvironment("Logging__LogLevel__Microsoft", "Information")
    .WithReference(appDb, "AppDb")
    .WaitFor(appDb);

builder.AddProject<BankingApp_Web>("Web")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(api)
    .WaitFor(api);

await builder.Build().RunAsync();