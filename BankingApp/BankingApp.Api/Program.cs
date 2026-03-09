using BankingApp.Infra;
using BankingApp.Infra.Contexts;
using BankingApp.Api.Configs;
using BankingApp.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfraServices();
builder.Services.AddSwaggerConfig();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(Assembly.Load("BankingApp.AppServices"));
    cfg.AddOpenBehavior(typeof(BankingApp.AppServices.Behaviors.ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(Assembly.Load("BankingApp.AppServices"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorWasm", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowBlazorWasm");
app.UseRouting();
app.UseSwaggerConfig();

var accountsGroup = app.MapGroup("/api/accounts").WithTags("Accounts");
AccountEndpoints.Map(accountsGroup);

var transactionsGroup = app.MapGroup("/api/transactions").WithTags("Transactions");
TransactionEndpoints.Map(transactionsGroup);

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    dbContext.Database.Migrate();
}

await app.RunAsync();