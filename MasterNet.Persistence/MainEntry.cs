using MasterNet.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(l =>
{
    l.ClearProviders();
});

services.AddDbContext<MasterNetDbContext>();

var provider = services.BuildServiceProvider();

try
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MasterNetDbContext>();
    await context.Database.MigrateAsync();

    Console.WriteLine("La migracion/sedding se ha realizado con exito");
}
catch (Exception ex)
{
    Console.WriteLine($"Error en el sedding/migracion {ex.Message}");
}