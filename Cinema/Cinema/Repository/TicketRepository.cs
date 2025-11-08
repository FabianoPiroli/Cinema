using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class TicketRepository : ITicketRepository
    {
         private readonly CinemaContext? _context;
        public TicketRepository(CinemaContext? context)
        {
            _context = context;
        }
        public async Task<List<Ticket>> GetAll()
        {
            return await _context.Tickets
                .Include(t => t.Session)
                .Include(t => t.Person)
                .ToListAsync();
        }

        public async Task<Ticket?> GetById(int id)
        {
            return await _context.Tickets
                .Include(t => t.Session)
                .Include(t => t.Person)
                .FirstOrDefaultAsync(t => t.ID == id);
        }

        public async Task<List<Ticket>> GetBySessionId(int sessionId)
        {
            return await _context.Tickets
                .Include(t => t.Session)
                .Include(t => t.Person)
                .Where(t => t.SessionID == sessionId)
                .ToListAsync();
        }

        public async Task Create(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
        }

    }
}
