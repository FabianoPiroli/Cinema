using System.Diagnostics;
using Cinema.Data;
using Cinema.Repository;
using Cinema.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly ISessionRepository _sessionRepository;

        public HomeController(
            ILogger<HomeController> logger, IMovieRepository movieRepository, ISessionRepository sessionRepository)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _sessionRepository = sessionRepository;
        }
        
        public async Task<IActionResult> Index()
        {
            var movies = await _movieRepository.GetAll();
            var sessions = await _sessionRepository.GetAll();
            
            ViewBag.Movies = movies;
            ViewBag.Sessions = sessions;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
