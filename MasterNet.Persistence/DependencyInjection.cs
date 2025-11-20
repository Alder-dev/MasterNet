using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace MasterNet.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MasterNetDbContext>(options =>
        {
            options.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information)
                   .EnableSensitiveDataLogging()
                   .EnableDetailedErrors();
            options.UseSqlite(configuration.GetConnectionString("SqliDatabase"))
            .UseAsyncSeeding(async (context, status, cancellationToken) =>
            {
                var masterNetDbContext = (MasterNetDbContext)context;
                var logger = context.GetService<ILogger<MasterNetDbContext>>();
                try
                {
                    await SeedDatabase.SeedRolesAndUsersAsync(context, logger, cancellationToken);
                    await SeedDatabase.SeedPreciosAsync(masterNetDbContext, logger, cancellationToken);
                    await SeedDatabase.SeedInstructoresAsync(masterNetDbContext, logger, cancellationToken);
                    await SeedDatabase.SeedCursosAsync(masterNetDbContext, logger, cancellationToken);
                    await SeedDatabase.SeedCalificacionesAsync(masterNetDbContext, logger, cancellationToken);
                }
                catch (Exception ex)
                {
                    logger?.LogError(ex, "Fallo cargando la data de precios");
                }
            });
        });

        return services;
    }
}