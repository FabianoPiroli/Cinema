using Cinema.Models;

namespace Cinema.Repository
{
    public interface IGenreRepository
    {
        public Task Create(Genre genre);
        public Task Update(Genre genre);
        public Task Delete(Genre genre);
    }
}
