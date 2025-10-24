using Cinema.Models;

namespace Cinema.Repository
{
    public interface ISessionRepository
    {
        public Task Create(Session session);
        public Task Update(Session session);
        public Task Delete(Session session);
    }
}
