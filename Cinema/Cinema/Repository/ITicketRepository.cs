using Cinema.Models;

namespace Cinema.Repository
{
    public interface ITicketRepository
    {
        public Task Create(Ticket ticket);
        public Task Update(Ticket ticket);
        public Task Delete(Ticket ticket);
    }
}
