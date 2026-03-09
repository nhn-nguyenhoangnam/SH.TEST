using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BankingApp.Web;
using BankingApp.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["services:Api:https:0"] 
                 ?? builder.Configuration["services:Api:http:0"]
                 ?? builder.Configuration["ApiBaseUrl"] 
                 ?? "http://localhost:5002";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<TransactionService>();

await builder.Build().RunAsync();