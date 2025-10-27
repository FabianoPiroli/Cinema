using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IMovieRepository _movieRepository;
        private readonly IRoomRepository _roomRepository;

        public SessionController(ISessionRepository sessionRepository, IMovieRepository movieRepository, IRoomRepository roomRepository)
        {
            _sessionRepository = sessionRepository;
            _movieRepository = movieRepository;
            _roomRepository = roomRepository;
        }

        public async Task<IActionResult> Index()
        {
            var sessions = await _sessionRepository.GetAll();
            return View(sessions);
        }

        public async Task<IActionResult> Details(int id)
        {
            var session = await _sessionRepository.GetById(id);
            if (session == null)
            {
                return NotFound();
            }
            return View(session);
        }
    }
}
