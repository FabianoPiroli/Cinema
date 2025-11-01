using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;


namespace Cinema.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ISessionRepository _sessionRepository;

        public MovieController(IMovieRepository movieRepository, ISessionRepository sessionRepository)
        {
            _movieRepository = movieRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAll();
            return View(movies);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie)
        {
            if(ModelState.IsValid)
            {
                await _movieRepository.Create(movie);
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }

            var sessions = await _sessionRepository.GetByMovieId(id);
            ViewBag.Sessions = sessions;
            return View(movie);
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var allMovies = await _movieRepository.GetAll();
            var filteredMovies = allMovies.Where(m => 
                m.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) == true ||
                m.Genres?.Any(g => g.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) == true) == true ||
                m.Actors?.Any(a => (a.FirstName + " " + a.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)) == true ||
                m.Directors?.Any(d => (d.FirstName + " " + d.LastName).Contains(query, StringComparison.OrdinalIgnoreCase)) == true
            ).ToList();

            ViewBag.SearchQuery = query;
            return View("Index", filteredMovies);
        }
    }
}
