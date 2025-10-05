using Cinema.Models;

namespace Cinema.Data
{
    public static class DbInitializer
    {
        public static void Initialize(CinemaContext context)
        {
            context.Database.EnsureCreated();
            // Look for any persons.
            if (context.Persons.Any())
            {
                return;   // DB has been seeded
            }
            var persons = new Person[]
            {
                new Person{FirstName="Carson", LastName="Alexander", BirthDate=DateTime.Parse("2005-09-01")},
                new Person{FirstName="Meredith", LastName="Alonso", BirthDate=DateTime.Parse("2002-09-01")},
                new Person{FirstName="Arturo", LastName="Anand", BirthDate=DateTime.Parse("2003-09-01")},
                new Person{FirstName="Gytis", LastName="Barzdukas", BirthDate=DateTime.Parse("2002-09-01")},
                new Person{FirstName="Yan", LastName="Li", BirthDate=DateTime.Parse("2002-09-01")},
                new Person{FirstName="Peggy", LastName="Justice", BirthDate=DateTime.Parse("2001-09-01")},
                new Person{FirstName="Laura", LastName="Norman", BirthDate=DateTime.Parse("2003-09-01")},
                new Person{FirstName="Nino", LastName="Olivetto", BirthDate=DateTime.Parse("2005-09-01")}
            };
            foreach (Person s in persons)
            {
                context.Persons.Add(s);
            }
            context.SaveChanges();
        }
    }
}
