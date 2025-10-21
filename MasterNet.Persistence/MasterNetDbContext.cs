using Bogus;
using MasterNet.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace MasterNet.Persistence;

public class MasterNetDbContext : DbContext
{
    public DbSet<Curso>? Cursos { get; set; }
    public DbSet<Instructor>? Instructores { get; set; }
    public DbSet<Precio>? Precios { get; set; }
    public DbSet<Calificacion>? Calificaciones { get; set; }

    public MasterNetDbContext() { }
    public MasterNetDbContext(DbContextOptions<MasterNetDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=MasterNet.db")
        .EnableDetailedErrors()
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .UseAsyncSeeding(async (context, status, cancellationToken) =>
        {
            var masterNetDbContext = (MasterNetDbContext)context;
            var logger = context.GetService<ILogger<MasterNetDbContext>>();
            try
            {
                await SeedDatabase.SeedPreciosAsync(masterNetDbContext, logger, cancellationToken);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Fallo cargando la data de precios");
            }
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Curso>().ToTable("cursos");
        modelBuilder.Entity<Instructor>().ToTable("instructores");
        modelBuilder.Entity<CursoInstructor>().ToTable("cursos_instructores");
        modelBuilder.Entity<Precio>().ToTable("precios");
        modelBuilder.Entity<CursoPrecio>().ToTable("cursos_precios");
        modelBuilder.Entity<Calificacion>().ToTable("calificaciones");
        modelBuilder.Entity<Photo>().ToTable("imagenes");

        modelBuilder.Entity<Precio>()
        .Property(p => p.Nombre)
        .HasColumnType("VARCHAR")
        .HasMaxLength(250);

        modelBuilder.Entity<Precio>()
        .Property(p => p.PrecioActual)
        .HasPrecision(10, 2);

        modelBuilder.Entity<Precio>()
        .Property(p => p.PrecioPromocion)
        .HasPrecision(10, 2);

        modelBuilder.Entity<Curso>()
        .HasMany(c => c.Photos)
        .WithOne(c => c.Curso)
        .HasForeignKey(c => c.CursoId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Curso>()
        .HasMany(c => c.Calificaciones)
        .WithOne(c => c.Curso)
        .HasForeignKey(c => c.CursoId)
        .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Curso>()
        .HasMany(c => c.Precios)
        .WithMany(c => c.Cursos)
        .UsingEntity<CursoPrecio>(
            j => j
            .HasOne(p => p.Precio)
            .WithMany(p => p.CursoPrecios)
            .HasForeignKey(p => p.PrecioId),
            j => j
            .HasOne(p => p.Curso)
            .WithMany(p => p.CursoPrecios)
            .HasForeignKey(p => p.CursoId),

            j =>
            {
                j.HasKey(pc => new { pc.CursoId, pc.PrecioId });
            }
        );

        modelBuilder.Entity<Curso>()
        .HasMany(c => c.Instructores)
        .WithMany(c => c.Cursos)
        .UsingEntity<CursoInstructor>(
            j => j
            .HasOne(i => i.Instructor)
            .WithMany(i => i.CursoInstructores)
            .HasForeignKey(i => i.InstructorId),
            j => j
            .HasOne(i => i.Curso)
            .WithMany(i => i.CursoInstructores)
            .HasForeignKey(i => i.CursoId),
            j =>
            {
                j.HasKey(ci => new { ci.CursoId, ci.InstructorId });
            }
        );
    }
}
