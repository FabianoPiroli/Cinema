using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class PersonRepository : IPersonRepository
    {
         private readonly CinemaContext? _context;
        public PersonRepository(CinemaContext? context)
        {
            _context = context;
        }
        public async Task Create(Person person)
        {
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Person person)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Person?>> Get(int personId)
        {
            var data = await _context.Persons
                    .Include(x => x.) //DONKEY
                    .Where(w => w.ID == personId)
                    .ToListAsync();
            return data.Cast<Person?>().ToList();
        }
        public async Task<List<Person?>> GetAll(int personId)
        {
            var data = await _context.Persons
                    .Include(x => x.)
                    .Where(w => w.ID == personId)
                    .ToListAsync();
            return data.Cast<Person?>().ToList();
        }
        public async Task Update(Person person)
        {
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }

    }
}
