using MasterNet.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MasterNet.Persistence;

public static class SeedDatabase
{
    public static async Task SeedPreciosAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Precios is null || dbContext.Precios.Any()) return;
            var jsonString = GetJsonFile("precios.json");
            var precios = JsonConvert.DeserializeObject<List<Precio>>(jsonString);

            if (precios?.Any() == false) return;

            dbContext.Precios.AddRange(precios!);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Fallo cargando la data de precios");
        }

    }
    
    private static string GetJsonFile(string fileName)
    {
        var leerForma1 = Path.Combine(
            Directory.GetCurrentDirectory(),
            "MasterNet.Persistence",
            "SeedData",
            fileName
        );

        var leerForma2 = Path.Combine(
            Directory.GetCurrentDirectory(),
            "SeedData",
            fileName
        );

        var leerForma3 = Path.Combine(
            AppContext.BaseDirectory,
            "SeedData",
            fileName
        );

        if (File.Exists(leerForma1)) return File.ReadAllText(leerForma1);
        if (File.Exists(leerForma2)) return File.ReadAllText(leerForma2);
        if (File.Exists(leerForma3)) return File.ReadAllText(leerForma3);
        return null!;
    }
}