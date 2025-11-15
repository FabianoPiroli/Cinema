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

        // Agora sempre carrega a lista de sessões (ViewBag.Sessions) e os clientes.
        // Se sessionId for fornecido, também preenche ViewBag.Session para mostrar detalhes da sessão selecionada.
        public async Task<IActionResult> Create(int? sessionId)
        {
            // Carrega todas as sessões para o dropdown
            var sessions = await _sessionRepository.GetAll();
            ViewBag.Sessions = sessions;

            // Carrega apenas clientes para o dropdown de clientes
            var clients = await _person_repository_getall_safe();
            var clientList = clients.Where(p => p.IsClient == true).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = $"{p.FirstName} {p.LastName}"
            }).ToList();
            ViewBag.Clients = clientList;

            if (sessionId.HasValue)
            {
                var session = await _sessionRepository.GetById(sessionId.Value);
                if (session == null)
                {
                    return NotFound();
                }

                ViewBag.Session = session;
            }
            else
            {
                ViewBag.Session = null;
            }

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

                await _ticket_repository_create_safe(ticket);
                return RedirectToAction(nameof(Index));
            }

            // Recarrega dados em caso de erro no ModelState: sessions e clients + sessão selecionada (se houver)
            var sessionsReload = await _sessionRepository.GetAll();
            ViewBag.Sessions = sessionsReload;

            var clientsReload = await _person_repository_getall_safe();
            var clientListReload = clientsReload.Where(p => p.IsClient == true).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = $"{p.FirstName} {p.LastName}"
            }).ToList();
            ViewBag.Clients = clientListReload;

            var session = await _sessionRepository.GetById(ticket.SessionID);
            if (session != null)
            {
                ViewBag.Session = session;
            }
            else
            {
                ViewBag.Session = null;
            }

            return View(ticket);
        }

        // Pequenas funções auxiliares para manter o código claro e evitar duplicação de lógica
        private async Task<List<Person>> _person_repository_getall_safe()
        {
            var clients = await _personRepository.GetAll();
            return clients ?? new List<Person>();
        }

        private async Task _ticket_repository_create_safe(Ticket ticket)
        {
            await _ticketRepository.Create(ticket);
        }
    }
}
