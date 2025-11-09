using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var rooms = await _roomRepository.GetAll();
            ViewBag.Rooms = new SelectList(rooms, "ID", "RoomNumber");

            var movies = await _movieRepository.GetAll();
            ViewBag.Movies = new SelectList(movies, "ID", "Title");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Session session)
        {
            if (ModelState.IsValid)
            {
                await _sessionRepository.Create(session);
                return RedirectToAction("Index");
            }

            // repopula selects antes de re-renderizar
            var rooms = await _roomRepository.GetAll();
            ViewBag.Rooms = new SelectList(rooms, "ID", "RoomNumber", session.RoomID);

            var movies = await _movieRepository.GetAll();
            ViewBag.Movies = new SelectList(movies, "ID", "Title", session.MovieID);

            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var session = await _sessionRepository.GetById(id);
            if (session == null) return NotFound();

            var rooms = await _roomRepository.GetAll();
            ViewBag.Rooms = new SelectList(rooms, "ID", "RoomNumber", session.RoomID);

            var movies = await _movieRepository.GetAll();
            ViewBag.Movies = new SelectList(movies, "ID", "Title", session.MovieID);

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Session session)
        {
            if (ModelState.IsValid)
            {
                await _sessionRepository.Update(session);
                return RedirectToAction("Index");
            }

            // repopula selects antes de re-renderizar
            var rooms = await _roomRepository.GetAll();
            ViewBag.Rooms = new SelectList(rooms, "ID", "RoomNumber", session.RoomID);

            var movies = await _movieRepository.GetAll();
            ViewBag.Movies = new SelectList(movies, "ID", "Title", session.MovieID);

            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _sessionRepository.GetById(id);
            if (session == null)
            {
                return NotFound();
            }
            await _sessionRepository.Delete(session);
            return RedirectToAction("Index");
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
