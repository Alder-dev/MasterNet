using MasterNet.Domain;
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
 
    // var newCurso = new Curso
    // {
    //     Id = Guid.NewGuid(),
    //     Titulo = "Curso de C#",
    //     Descripcion = "Curso de C# para principiantes con MASTER.NET",
    //     FechaPublicacion = DateTime.Now
    // };

    // context.Cursos!.Add(newCurso);
    // await context.SaveChangesAsync();

    // var cursos = await context.Cursos!.ToListAsync();

    // Console.WriteLine($"Cantidad de cursos: {cursos.Count}");

    // foreach (var curso in cursos)
    // {
    //     Console.WriteLine($"{curso.Id} - {curso.Titulo}");
    // }
}
catch (Exception ex)
{
    Console.WriteLine($"Error en el sedding/migracion {ex.Message}");
}