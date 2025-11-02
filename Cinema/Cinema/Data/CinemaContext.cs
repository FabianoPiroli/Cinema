using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Cinema.Models;

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

        modelBuilder.Entity<Person>().ToTable("Person");
    }
}
