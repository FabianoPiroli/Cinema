using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class RoleRepository : IRoleRepository
    {
         private readonly CinemaContext? _context;
        public RoleRepository(CinemaContext? context)
        {
            _context = context;
        }
        public async Task Create(Role role)
        {
            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Role role)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Role role)
        {
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

    }
}
