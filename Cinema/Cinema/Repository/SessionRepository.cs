using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class SessionRepository : ISessionRepository
    {
         private readonly CinemaContext? _context;
        public SessionRepository(CinemaContext? context)
        {
            _context = context;
        }
        public async Task<List<Session>> GetAll()
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.Tickets)
                .ToListAsync();
        }

        public async Task<Session?> GetById(int id)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.Tickets)
                .FirstOrDefaultAsync(s => s.ID == id);
        }

        public async Task<List<Session>> GetByMovieId(int movieId)
        {
            return await _context.Sessions
                .Include(s => s.Movie)
                .Include(s => s.Room)
                .Include(s => s.Tickets)
                .Where(s => s.MovieID == movieId)
                .ToListAsync();
        }

        public async Task Create(Session session)
        {
            await _context.Sessions.AddAsync(session);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Session session)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Session session)
        {
            _context.Sessions.Update(session);
            await _context.SaveChangesAsync();
        }

    }
}
