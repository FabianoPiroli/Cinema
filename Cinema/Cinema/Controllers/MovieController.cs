using Cinema.Models;
using Cinema.Repository;
using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPersonRepository _personRepository;
        private readonly CinemaContext _context;

        public MovieController(
            IMovieRepository movieRepository,
            ISessionRepository sessionRepository,
            IPersonRepository personRepository,
            CinemaContext context)
        {
            _movieRepository = movieRepository;
            _sessionRepository = sessionRepository;
            _personRepository = personRepository; // preserva nome original se existir; caso contrário use _personRepository
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAll();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var persons = await _personRepository.GetAll();
            ViewBag.Actors = persons
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();
            ViewBag.Directors = persons
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            // popula ViewBag.AgeRating (nome deve bater com a view)
            var ageRatings = await _context.Set<AgeRating>()
                .OrderBy(a => a.ID)
                .ToListAsync();
            ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, int[] SelectedDirectorIds, int? AgeRatingId)
        {
            // associa diretores selecionados
            if (SelectedDirectorIds != null && SelectedDirectorIds.Length > 0)
            {
                var tasks = SelectedDirectorIds.Select(id => _personRepository.GetById(id)).ToArray();
                var persons = await Task.WhenAll(tasks);
                movie.Directors = persons.Where(p => p != null).Select(p => p!).ToList();
            }

            // associa AgeRating selecionado
            if (AgeRatingId.HasValue)
            {
                var rating = await _context.Set<AgeRating>().FindAsync(AgeRatingId.Value);
                if (rating != null) movie.AgeRating = rating;
            }
            else
            {
                // tentativa alternativa: ler do form caso o name seja diferente
                if (int.TryParse(Request.Form["AgeRating"].ToString(), out var parsed))
                {
                    var rating = await _context.Set<AgeRating>().FindAsync(parsed);
                    if (rating != null) movie.AgeRating = rating;
                }
            }

            if (ModelState.IsValid)
            {
                await _movieRepository.Create(movie);
                return RedirectToAction("Index");
            }

            // repopula ViewBag em caso de erro para re-renderizar a view
            var all = await _personRepository.GetAll();
            ViewBag.Actors = all
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();
            ViewBag.Directors = all
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            var ageRatings = await _context.Set<AgeRating>()
                .OrderBy(a => a.ID)
                .ToListAsync();
            ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating");

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
