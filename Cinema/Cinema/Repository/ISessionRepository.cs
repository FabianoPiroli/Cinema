using Cinema.Models;

namespace Cinema.Repository
{
    public interface ISessionRepository
    {
        public Task<List<Session>> GetAll();
        public Task<Session?> GetById(int id);
        public Task<List<Session>> GetByMovieId(int movieId);
        public Task Create(Session session);
        public Task Update(Session session);
        public Task Delete(Session session);
    }
}
