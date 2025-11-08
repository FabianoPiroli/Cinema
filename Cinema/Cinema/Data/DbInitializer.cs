using Cinema.Models;
using System.Linq;

namespace Cinema.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CinemaContext context)
        {
            // NÃO chamar EnsureCreated() quando usamos migrations. O Program.cs já chama Database.Migrate().

            // Seed AgeRating se vazio
            // Usar context.Set<AgeRating>() para não depender de uma propriedade DbSet chamada AgeRating
            if (!context.Set<AgeRating>().Any())
            {
                var ages = new AgeRating[]
                {
                    new AgeRating { Rating = "Livre" },
                    new AgeRating { Rating = "10" },
                    new AgeRating { Rating = "12" },
                    new AgeRating { Rating = "14" },
                    new AgeRating { Rating = "16" },
                    new AgeRating { Rating = "18" }
                };
                context.Set<AgeRating>().AddRange(ages);
                context.SaveChanges();
            }

            // Seed Roles se vazio (Cliente, Ator, Diretor, Estudante)
            if (!context.Roles.Any())
            {
                var roles = new Role[]
                {
                    new Role { Name = "Cliente" },
                    new Role { Name = "Ator" },
                    new Role { Name = "Diretor" },
                    new Role { Name = "Estudante" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            // Look for any persons.
            if (context.Persons.Any())
            {
                return;   // DB has been seeded
            }

            var persons = new Person[]
            {
                new Person{FirstName="Carson", LastName="Alexander", BirthDate=DateOnly.Parse("2005-09-01")},
                new Person{FirstName="Meredith", LastName="Alonso", BirthDate=DateOnly.Parse("2002-09-01")},
                new Person{FirstName="Arturo", LastName="Anand", BirthDate=DateOnly.Parse("2003-09-01")},
                new Person{FirstName="Gytis", LastName="Barzdukas", BirthDate=DateOnly.Parse("2002-09-01")},
                new Person{FirstName="Yan", LastName="Li", BirthDate=DateOnly.Parse("2002-09-01")},
                new Person{FirstName="Peggy", LastName="Justice", BirthDate=DateOnly.Parse("2001-09-01")},
                new Person{FirstName="Laura", LastName="Norman", BirthDate=DateOnly.Parse("2003-09-01")},
                new Person{FirstName="Nino", LastName="Olivetto", BirthDate=DateOnly.Parse("2005-09-01")}
            };

            foreach (var p in persons)
            {
                bool exists = context.Persons.Any(x =>
                    x.FirstName == p.FirstName &&
                    x.LastName == p.LastName &&
                    x.BirthDate == p.BirthDate);
                if (!exists)
                {
                    context.Persons.Add(p);
                }
            }

            context.SaveChanges();
        }
    }
}