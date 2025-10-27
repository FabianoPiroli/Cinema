using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly CinemaContext _context;

        public PersonRepository(CinemaContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Create(Person person)
        {
            if (person is null) throw new ArgumentNullException(nameof(person));
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Person person)
        {
            if (person is null) throw new ArgumentNullException(nameof(person));
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();
        }

        // Retorna um único registro por ID (método auxiliar, opcional se a interface não exigir)
        public async Task<Person?> GetById(int personId)
        {
            return await _context.Persons
                .Include(p => p.role) // carrega a role se existir
                .FirstOrDefaultAsync(p => p.ID == personId);
        }

        // Retorna todos as pessoas
        public async Task<List<Person>> GetAll()
        {
            return await _context.Persons
                .Include(p => p.role)
                .ToListAsync();
        }

        public async Task Update(Person person)
        {
            if (person is null) throw new ArgumentNullException(nameof(person));
            _context.Persons.Update(person);
            await _context.SaveChangesAsync();
        }
    }
}
