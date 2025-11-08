using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Repository
{
    public class MovieRepository : IMovieRepository
    {
         private readonly CinemaContext? _context;
        public MovieRepository(CinemaContext? context)
        {
            _context = context;
        }
        public async Task<List<Movie>> GetAll()
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.AgeRating) // <--- incluir AgeRating aqui
                .ToListAsync();
        }

        public async Task<Movie?> GetById(int id)
        {
            return await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .Include(m => m.AgeRating) // <--- e aqui
                .FirstOrDefaultAsync(m => m.ID == id);
        }

        public async Task Create(Movie movie)
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

    }
}