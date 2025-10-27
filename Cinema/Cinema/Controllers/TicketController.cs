using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISessionRepository _sessionRepository;

        public TicketController(ITicketRepository ticketRepository, ISessionRepository sessionRepository)
        {
            _ticketRepository = ticketRepository;
            _sessionRepository = sessionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketRepository.GetAll();
            return View(tickets);
        }

        public async Task<IActionResult> Create(int sessionId)
        {
            var session = await _sessionRepository.GetById(sessionId);
            if (session == null)
            {
                return NotFound();
            }

            ViewBag.Session = session;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.PurchaseDate = DateTime.Now;
                await _ticketRepository.Create(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }
    }
}
