using Cinema.Models;

namespace Cinema.Repository
{
    public interface IMovieRepository
    {
        public Task<List<Movie>> GetAll();
        public Task<Movie?> GetById(int id);
        public Task Create(Movie movie);
        public Task Update(Movie movie);
        public Task Delete(Movie movie);
    }
}
