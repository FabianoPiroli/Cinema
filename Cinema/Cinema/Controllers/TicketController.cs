using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cinema.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPersonRepository _personRepository;

        public TicketController(ITicketRepository ticketRepository, ISessionRepository sessionRepository, IPersonRepository personRepository)
        {
            _ticketRepository = ticketRepository;
            _sessionRepository = sessionRepository;
            _personRepository = personRepository;
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

            // Carrega apenas clientes para o dropdown
            var clients = await _personRepository.GetAll();
            var clientList = clients.Where(p => p.IsClient == true).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = $"{p.FirstName} {p.LastName}"
            }).ToList();

            ViewBag.Session = session;
            ViewBag.Clients = clientList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.PurchaseDate = DateTime.Now;
                
                // Aplica desconto de estudante se necessário
                if (ticket.PersonID.HasValue)
                {
                    var person = await _personRepository.GetById(ticket.PersonID.Value);
                    if (person != null && person.IsStudent == true && ticket.Price.HasValue)
                    {
                        ticket.Price = ticket.StudentPrice(ticket.Price.Value);
                    }
                }
                
                await _ticketRepository.Create(ticket);
                return RedirectToAction(nameof(Index));
            }
            
            // Recarrega dados em caso de erro
            var session = await _sessionRepository.GetById(ticket.SessionID);
            if (session != null)
            {
                var clients = await _personRepository.GetAll();
                var clientList = clients.Where(p => p.IsClient == true).Select(p => new SelectListItem
                {
                    Value = p.ID.ToString(),
                    Text = $"{p.FirstName} {p.LastName}"
                }).ToList();
                
                ViewBag.Session = session;
                ViewBag.Clients = clientList;
            }
            
            return View(ticket);
        }
    }
}
