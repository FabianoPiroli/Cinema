using Cinema.Models;
using Cinema.Repository;
using Cinema.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QRCoder;
using System.Text;

namespace Cinema.Controllers
{
    public class TicketController : Controller
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IPersonRepository _personRepository;
        private readonly CinemaContext _context;

        public TicketController(ITicketRepository ticketRepository, ISessionRepository sessionRepository, IPersonRepository personRepository, CinemaContext context)
        {
            _ticketRepository = ticketRepository;
            _sessionRepository = sessionRepository;
            _personRepository = personRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketRepository.GetAll();
            return View(tickets);
        }
        
        [HttpGet]
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
                    var person = await _person_repository_getbyid_safe(ticket.PersonID.Value);
                    if (person != null && person.IsStudent == true && ticket.Price.HasValue)
                    {
                        ticket.Price = ticket.StudentPrice(ticket.Price.Value);
                    }
                }

                await _ticket_repository_create_safe(ticket);
                return RedirectToAction(nameof(Index));
            }

            // Recarrega dropdowns em caso de erro
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
            ViewBag.Session = session;

            return View(ticket);
        }

        private async Task<List<Person>> _person_repository_getall_safe()
        {
            var clients = await _personRepository.GetAll();
            return clients ?? new List<Person>();
        }

        private async Task<Person?> _person_repository_getbyid_safe(int id)
        {
            return await _personRepository.GetById(id);
        }

        private async Task _ticket_repository_create_safe(Ticket ticket)
        {
            await _ticketRepository.Create(ticket);
        }

        // Editar ingresso
        [HttpGet]
        public async Task<IActionResult> Update(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ticket = await _ticketRepository.GetById(id.Value);
            if (ticket == null)
                return NotFound();

            // Carrega sessões e clients para os dropdowns
            var sessions = await _sessionRepository.GetAll();
            ViewBag.Sessions = sessions;

            var clients = await _person_repository_getall_safe();
            var clientList = clients.Where(p => p.IsClient == true).Select(p => new SelectListItem
            {
                Value = p.ID.ToString(),
                Text = $"{p.FirstName} {p.LastName}"
            }).ToList();
            ViewBag.Clients = clientList;

            // Session selecionada (para exibir detalhes)
            ViewBag.Session = ticket.Session;

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, Ticket ticket)
        {
            if (id != ticket.ID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                // Recupera o registro existente para preservar campos que não devem ser sobrescritos
                var existing = await _ticketRepository.GetById(id);
                if (existing == null)
                    return NotFound();

                // Atualiza os campos editáveis
                existing.SessionID = ticket.SessionID;
                existing.PersonID = ticket.PersonID;
                existing.Price = ticket.Price;

                // Preserva PurchaseDate (não sobrescreve). Caso queira atualizar, faça explicitamente.
                // Reaplica desconto de estudante se necessário
                if (existing.PersonID.HasValue)
                {
                    var person = await _person_repository_getbyid_safe(existing.PersonID.Value);
                    if (person != null && person.IsStudent == true && existing.Price.HasValue)
                    {
                        existing.Price = existing.StudentPrice(existing.Price.Value);
                    }
                }

                await _ticketRepository.Update(existing);
                return RedirectToAction(nameof(Index));
            }

            // Recarrega dropdowns em caso de erro
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
            ViewBag.Session = session;

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketRepository.GetById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            await _ticketRepository.Delete(ticket);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Print(int? id)
        {
            if (!id.HasValue)
                return BadRequest();

            var ticket = await _ticketRepository.GetById(id.Value);
            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        //Gera QR code para o ingresso
        [HttpGet]
        public async Task<IActionResult> QrCode(int? id, int pixelsPerModule = 4)
        {
            if (!id.HasValue)
                return BadRequest();

            var ticket = await _ticketRepository.GetById(id.Value);
            if (ticket == null)
                return NotFound();

            var payload = new
            {
                ticketId = ticket.ID,
                sessionId = ticket.SessionID,
                purchaseDate = ticket.PurchaseDate.ToString("o")
            };

            var payloadString = System.Text.Json.JsonSerializer.Serialize(payload);

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(payloadString, QRCodeGenerator.ECCLevel.Q);
            var svg = new SvgQRCode(qrData).GetGraphic(pixelsPerModule);

            return Content(svg, "image/svg+xml");
        }
    }
}
