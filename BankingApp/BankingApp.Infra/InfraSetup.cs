using BankingApp.AppServices;
using BankingApp.AppServices.Features.Idempotency;
using BankingApp.Infra.Contexts;
using BankingApp.Infra.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BankingApp.Infra;

public static class InfraSetup
{
    private static IServiceCollection AddImplementations(this IServiceCollection services)
    {
        services.Scan(scan =>
            scan.FromAssemblies(typeof(InfraSetup).Assembly)
                .AddClasses(c =>
                    c.AssignableTo(typeof(IRepository<>)), false).AsImplementedInterfaces()
                .AsImplementedInterfaces()
        );

        return services;
    }

    public static IServiceCollection AddInfraServices(this IServiceCollection service)
    {
        service.AddDbContext<CoreDbContext>((sp, opt) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            var conn = config.GetConnectionString("AppDb")!;

            if (string.IsNullOrWhiteSpace(conn))
                throw new InvalidOperationException(
                    $"Database `AppDb` connection string is not configured.");

            opt.UseSqlWithMigration(conn);
        })
        .AddMemoryCache()
        .AddScoped<IUnitOfWork, UnitOfWork>()
        .AddScoped<IIdempotencyService, InMemoryIdempotencyService>()
        .AddImplementations();

        return service;
    }

    private static DbContextOptionsBuilder UseSqlWithMigration(this DbContextOptionsBuilder builder,
        string connectionString)
    {
        builder.ConfigureWarnings(warnings =>
        {
            warnings.Log(RelationalEventId.PendingModelChangesWarning);
            warnings.Log(CoreEventId.ManyServiceProvidersCreatedWarning);
        });
#if DEBUG
        builder.EnableDetailedErrors().EnableSensitiveDataLogging();
#endif

        return builder
            .UseSqlServer(connectionString,
                o => o
                    .MinBatchSize(1)
                    .MaxBatchSize(100)
                    .MigrationsHistoryTable(nameof(CoreDbContext), "Migrations")
                    .MigrationsAssembly(typeof(CoreDbContext).Assembly)
                    .EnableRetryOnFailure()
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }
}