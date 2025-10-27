using Cinema.Models;

namespace Cinema.Repository
{
    public interface ITicketRepository
    {
        public Task<List<Ticket>> GetAll();
        public Task<Ticket?> GetById(int id);
        public Task<List<Ticket>> GetBySessionId(int sessionId);
        public Task Create(Ticket ticket);
        public Task Update(Ticket ticket);
        public Task Delete(Ticket ticket);
    }
}
