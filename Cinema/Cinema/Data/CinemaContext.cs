using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Cinema.Models;
using System.Linq;
using System;

public class CinemaContext : DbContext
{
    public CinemaContext (DbContextOptions<CinemaContext> options)
        : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<AgeRating> AgeRatings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Conversor DateOnly <-> DateTime (armazena como Date no DB)
        var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
            d => d.ToDateTime(TimeOnly.MinValue),
            dt => DateOnly.FromDateTime(dt));

        // Aplica globalmente a todas propriedades DateOnly
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (clrType == null) continue;

            var props = clrType.GetProperties()
                .Where(p => p.PropertyType == typeof(DateOnly));
            foreach (var prop in props)
            {
                modelBuilder.Entity(clrType)
                    .Property(prop.Name)
                    .HasConversion(dateOnlyConverter)
                    .HasColumnType("date");
            }
        }

        // manter nome de tabela Person (opcional)
        modelBuilder.Entity<Person>().ToTable("Person");

        // configurar many-to-many: Movie <-> Genre (MovieGenres)
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Genres)
            .WithMany(g => g.Movies)
            .UsingEntity(j => j.ToTable("MovieGenres"));

        // configurar many-to-many: Movie <-> Person (Actors) -> MovieActors
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Actors)
            .WithMany(p => p.ActedMovies)
            .UsingEntity(j => j.ToTable("MovieActors"));

        // configurar many-to-many: Movie <-> Person (Directors) -> MovieDirectors
        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Directors)
            .WithMany(p => p.DirectedMovies)
            .UsingEntity(j => j.ToTable("MovieDirectors"));
    }
}
