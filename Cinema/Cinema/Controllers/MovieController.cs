using Cinema.Models;
using Cinema.Repository;
using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cinema.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPersonRepository _personRepository;
        private readonly CinemaContext _context;
        private readonly IWebHostEnvironment _env;

        public MovieController(
            IMovieRepository movieRepository,
            ISessionRepository sessionRepository,
            IPersonRepository personRepository,
            CinemaContext context,
            IWebHostEnvironment env)
        {
            _movieRepository = movieRepository;
            _sessionRepository = sessionRepository;
            _personRepository = personRepository;
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAll();
            return View(movies);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var persons = await _personRepository.GetAll() ?? Enumerable.Empty<Person>();

            ViewBag.Actors = persons
                .Where(p => p.IsActor)
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            ViewBag.Directors = persons
                .Where(p => p.IsDirector)
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            var ageRatings = await _context.Set<AgeRating>()
                .OrderBy(a => a.ID)
                .ToListAsync();
            ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating");

            var genres = await _context.Set<Genre>()
                .OrderBy(g => g.ID)
                .ToListAsync();
            ViewBag.Genres = new SelectList(genres, "ID", "Name");

            // garante que a view receba um modelo (evita problemas de metadata quando Model == null)
            return View(new Movie());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, int[] SelectedDirectorIds, int[] SelectedActorIds, int? AgeRatingId, int[] SelectedGenreIds, IFormFile? CoverImage)
        {
            // associa atores selecionados
            if (SelectedActorIds?.Length > 0)
            {
                var actors = await _context.Persons
                    .Include(p => p.Roles)
                    .Where(p => SelectedActorIds.Contains(p.ID))
                    .ToListAsync();
                movie.Actors = actors;
            }
            // associa diretores selecionados
            if (SelectedDirectorIds?.Length > 0)
            {
                var directors = await _context.Persons
                    .Include(p => p.Roles)
                    .Where(p => SelectedDirectorIds.Contains(p.ID))
                    .ToListAsync();
                movie.Directors = directors;
            }

            // associa AgeRating selecionado
            if (AgeRatingId.HasValue)
            {
                var rating = await _context.Set<AgeRating>().FindAsync(AgeRatingId.Value);
                if (rating != null) movie.AgeRating = rating;
            }
            else
            {
                if (int.TryParse(Request.Form["AgeRating"].ToString(), out var parsed))
                {
                    var rating = await _context.Set<AgeRating>().FindAsync(parsed);
                    if (rating != null) movie.AgeRating = rating;
                }
            }

            // associa generos selecionados
            if (SelectedGenreIds != null && SelectedGenreIds.Length > 0)
            {
                var selectedGenres = await _context.Genres
                    .Where(g => SelectedGenreIds.Contains(g.ID))
                    .ToListAsync();
                movie.Genres = selectedGenres;
            }

            // --- salvar arquivo em wwwroot/uploads (se houver) ---
            var file = CoverImage ?? Request.Form.Files["CoverImage"] ?? Request.Form.Files.FirstOrDefault();
            if (file != null && file.Length > 0)
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
                if (!allowed.Contains(file.ContentType))
                {
                    // colocar erro no nível do modelo para aparecer no summary
                    ModelState.AddModelError(string.Empty, "Tipo de arquivo não permitido. Tipos permitidos: JPG, PNG, GIF.");
                }
                else if (file.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError(string.Empty, "Arquivo muito grande (máx 5MB).");
                }
                else
                {
                    var ext = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }
                    movie.ImageURL = "/uploads/" + fileName;
                }
            }

            if (ModelState.IsValid)
            {
                await _movieRepository.Create(movie);
                return RedirectToAction("Index");
            }

            // repopula ViewBag em caso de erro para re-renderizar a view
            var all = await _personRepository.GetAll() ?? Enumerable.Empty<Person>();

            ViewBag.Actors = all
                .Where(p => p.IsActor)
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            ViewBag.Directors = all
                .Where(p => p.IsDirector)
                .Select(p => new SelectListItem { Value = p.ID.ToString(), Text = (p.FirstName + " " + p.LastName).Trim() })
                .ToList();

            var ageRatings = await _context.Set<AgeRating>()
                .OrderBy(a => a.ID)
                .ToListAsync();
            ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating");

            var genres = await _context.Set<Genre>()
                .OrderBy(g => g.ID)
                .ToListAsync();
            ViewBag.Genres = new SelectList(genres, "ID", "Name");

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var movie = await _movieRepository.GetById(id);
            if (movie == null) return NotFound();

            await _movieRepository.Delete(movie);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var movie = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (movie == null) return NotFound();

            var persons = await _personRepository.GetAll() ?? Enumerable.Empty<Person>();

            ViewBag.Actors = persons
                .Where(p => p.IsActor)
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = (p.FirstName + " " + p.LastName).Trim(),
                    Selected = movie.Actors?.Any(a => a.ID == p.ID) == true
                })
                .ToList();

            ViewBag.Directors = persons
                .Where(p => p.IsDirector)
                .Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = (p.FirstName + " " + p.LastName).Trim(),
                    Selected = movie.Directors?.Any(d => d.ID == p.ID) == true
                })
                .ToList();

            var ageRatings = await _context.Set<AgeRating>()
                .OrderBy(a => a.ID)
                .ToListAsync();
            ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating", movie.AgeRatingID);

            // marcar gêneros selecionados
            var genres = await _context.Set<Genre>()
                .OrderBy(g => g.ID)
                .ToListAsync();
            ViewBag.Genres = genres
                .Select(g => new SelectListItem
                {
                    Value = g.ID.ToString(),
                    Text = g.Name,
                    Selected = movie.Genres?.Any(mg => mg.ID == g.ID) == true
                })
                .ToList();

            return View(movie);
        }

        // --- Adicionado: Update POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Movie movie, int[] SelectedDirectorIds, int[] SelectedActorIds, int? AgeRatingId, int[] SelectedGenreIds, IFormFile? CoverImage)
        {
            if (!ModelState.IsValid)
            {
                var personsEmpty = await _personRepository.GetAll() ?? Enumerable.Empty<Person>();

                ViewBag.Actors = personsEmpty.Where(p => p.IsActor)
                    .Select(p => new SelectListItem
                    {
                        Value = p.ID.ToString(),
                        Text = (p.FirstName + " " + p.LastName).Trim(),
                        Selected = SelectedActorIds != null && SelectedActorIds.Contains(p.ID)
                    })
                    .ToList();

                ViewBag.Directors = personsEmpty.Where(p => p.IsDirector)
                    .Select(p => new SelectListItem
                    {
                        Value = p.ID.ToString(),
                        Text = (p.FirstName + " " + p.LastName).Trim(),
                        Selected = SelectedDirectorIds != null && SelectedDirectorIds.Contains(p.ID)
                    })
                    .ToList();

                var ageRatings = await _context.Set<AgeRating>().OrderBy(a => a.ID).ToListAsync();
                ViewBag.AgeRating = new SelectList(ageRatings, "ID", "Rating", AgeRatingId);

                var genres = await _context.Set<Genre>().OrderBy(g => g.ID).ToListAsync();
                ViewBag.Genres = genres
                    .Select(g => new SelectListItem
                    {
                        Value = g.ID.ToString(),
                        Text = g.Name,
                        Selected = SelectedGenreIds != null && SelectedGenreIds.Contains(g.ID)
                    })
                    .ToList();

                return View(movie);
            }

            var existing = await _context.Movies
                .Include(m => m.Genres)
                .Include(m => m.Actors)
                .Include(m => m.Directors)
                .FirstOrDefaultAsync(m => m.ID == movie.ID);

            if (existing == null) return NotFound();

            // atualizar campos escalares
            existing.Title = movie.Title;
            existing.DurationInMinutes = movie.DurationInMinutes;
            existing.Synopsis = movie.Synopsis; // <-- ADICIONADO: garantir que a sinopse seja atualizada

            // age rating
            if (AgeRatingId.HasValue)
            {
                var rating = await _context.Set<AgeRating>().FindAsync(AgeRatingId.Value);
                existing.AgeRating = rating;
                existing.AgeRatingID = rating?.ID;
            }
            else
            {
                existing.AgeRating = null;
                existing.AgeRatingID = null;
            }

            // atualizar atores
            existing.Actors = new List<Person>();
            if (SelectedActorIds?.Length > 0)
            {
                var actors = await _context.Persons.Where(p => SelectedActorIds.Contains(p.ID)).ToListAsync();
                existing.Actors.AddRange(actors);
            }

            // atualizar diretores
            existing.Directors = new List<Person>();
            if (SelectedDirectorIds?.Length > 0)
            {
                var directors = await _context.Persons.Where(p => SelectedDirectorIds.Contains(p.ID)).ToListAsync();
                existing.Directors.AddRange(directors);
            }

            // atualizar generos
            existing.Genres = new List<Genre>();
            if (SelectedGenreIds?.Length > 0)
            {
                var genres = await _context.Genres.Where(g => SelectedGenreIds.Contains(g.ID)).ToListAsync();
                existing.Genres.AddRange(genres);
            }

            // --- salvar novo arquivo se enviado ---
            if (CoverImage != null && CoverImage.Length > 0)
            {
                var allowed = new[] { "image/jpeg", "image/png", "image/gif" };
                if (!allowed.Contains(CoverImage.ContentType))
                {
                    ModelState.AddModelError("CoverImage", "Tipo de arquivo não permitido.");
                }
                else if (CoverImage.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("CoverImage", "Arquivo muito grande (máx 5MB).");
                }
                else
                {
                    // tenta remover arquivo antigo (somente se estiver em /uploads/)
                    try
                    {
                        if (!string.IsNullOrEmpty(existing.ImageURL) && existing.ImageURL.StartsWith("/uploads/"))
                        {
                            var oldPath = Path.Combine(_env.WebRootPath, existing.ImageURL.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldPath))
                            {
                                System.IO.File.Delete(oldPath);
                            }
                        }
                    }
                    catch { /* ignore erros ao deletar arquivo antigo */ }

                    var ext = Path.GetExtension(CoverImage.FileName);
                    var fileName = $"{Guid.NewGuid()}{ext}";
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await CoverImage.CopyToAsync(fs);
                    }
                    existing.ImageURL = "/uploads/" + fileName;
                }
            }

            // salvar
            await _movieRepository.Update(existing);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query)) return RedirectToAction(nameof(Index));

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

        public async Task<IActionResult> Details(int id)
        {
            var movie = await _movieRepository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }
    }
}
