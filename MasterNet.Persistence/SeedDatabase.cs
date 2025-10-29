using System.Collections.Frozen;
using MasterNet.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

            if (jsonString is null) return;

            var precios = JsonConvert.DeserializeObject<List<Precio>>(jsonString);

            if (precios is null || precios.Any() == false) return;

            dbContext.Precios.AddRange(precios!);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Fallo cargando la data de precios");
        }

    }

    public static async Task SeedInstructoresAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Instructores is null || dbContext.Instructores.Any()) return;
            var jsonString = GetJsonFile("instructores.json");

            if (jsonString is null) return;
            var instructores = JsonConvert.DeserializeObject<List<Instructor>>(jsonString);

            if (instructores is null || instructores.Any() == false) return;

            dbContext.Instructores.AddRange(instructores!);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Fallo cargando la data de Instructores");
        }

    }

    public static async Task SeedCursosAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Cursos is null || dbContext.Cursos.Any()) return;
            var jsonString = GetJsonFile("cursos.json");

            if (jsonString is null) return;

            var instructores = dbContext.Instructores!.ToFrozenDictionary(i => i.Id, i => i);
            var precios = dbContext.Precios!.ToFrozenDictionary(p => p.Id, p => p);

            var arrayCursos = JArray.Parse(jsonString);

            var cursosDb = new List<Curso>();

            foreach (var obj in arrayCursos)
            {
                var idString = obj["Id"]?.ToString();
                if (!Guid.TryParse(idString, out var id)) id = Guid.NewGuid();

                var titulo = obj["Titulo"]?.ToString();
                var descripcion = obj["Descripcion"]?.ToString();

                DateTime? fechaPublicacion = null;
                var fechaPublicacionStr = obj["FechaPublicacion"]?.ToString();

                if (!string.IsNullOrWhiteSpace(fechaPublicacionStr) && DateTime.TryParse(fechaPublicacionStr, out var fp))
                {
                    fechaPublicacion = fp;
                }

                var curso = new Curso
                {
                    Id = id,
                    Titulo = titulo!,
                    Descripcion = descripcion!,
                    FechaPublicacion = fechaPublicacion,
                    Calificaciones = new List<Calificacion>(),
                    Precios = new List<Precio>(),
                    CursoPrecios = new List<CursoPrecio>(),
                    Instructores = new List<Instructor>(),
                    CursoInstructores = new List<CursoInstructor>(),
                    Photos = new List<Photo>()
                };

                if (obj["Precios"] is JArray preciosC)
                {
                    foreach (var pid in preciosC)
                    {
                        var idt = new Guid(pid?.ToString()!);
                        if (precios.TryGetValue(idt, out var precio))
                        {
                            curso.Precios.Add(precio);
                        }
                    }
                }

                if (obj["Instructores"] is JArray instructoresC)
                {
                    foreach (var iid in instructoresC)
                    {
                        var idt = new Guid(iid?.ToString()!);
                        if (instructores.TryGetValue(idt, out var instructor))
                        {
                            curso.Instructores.Add(instructor);
                        }
                    }
                }

                cursosDb.Add(curso);
            }

            await dbContext.Cursos.AddRangeAsync(cursosDb);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Fallo cargando la data de Cursos");
        }

    }

    public static async Task SeedCalificacionesAsync(
        MasterNetDbContext dbContext,
        ILogger? logger,
        CancellationToken cancellationToken
    )
    {
        try
        {
            if (dbContext.Calificaciones is null || dbContext.Calificaciones.Any()) return;
            var jsonString = GetJsonFile("calificaciones.json");

            if (jsonString is null) return;
            var calificaciones = JsonConvert.DeserializeObject<List<Calificacion>>(jsonString);

            if (calificaciones is null || calificaciones.Any() == false) return;

            foreach (var ca in calificaciones!)
            {
                ca.Curso = null;
            }

            dbContext.Calificaciones.AddRange(calificaciones!);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger?.LogWarning(ex, "Fallo cargando la data de Calificaciones");
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