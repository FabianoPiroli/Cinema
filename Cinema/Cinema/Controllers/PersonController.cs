using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Linq;

namespace Cinema.Controllers
{
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;
        private readonly IPersonRepository _personRepository;
        private readonly CinemaContext _context;

        public PersonController(ILogger<PersonController> logger,
                                IPersonRepository personRepository,
                                CinemaContext context)
        {
            _logger = logger;
            _personRepository = personRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var persons = await _personRepository.GetAll();
            return View(persons);
        }

        // Popula ViewBag.RolesList com todas as roles do banco; optionally passar selected ids
        private async Task PopulateRolesAsync(IEnumerable<int>? selectedRoleIds = null)
        {
            var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            ViewBag.RolesList = roles;
            // também deixo uma SelectList caso alguma view precise do select único
            ViewBag.role = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(roles, "ID", "Name", selectedRoleIds);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateRolesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Person person)
        {
            var selected = Request.Form["selectedRoles"].ToArray();
            var selectedIds = selected.Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                                      .Where(id => id.HasValue)
                                      .Select(id => id!.Value)
                                      .ToList();

            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync(selectedIds);
                return View(person);
            }

            // vincula roles selecionadas (ou vazia)
            if (selectedIds.Any())
            {
                var roles = await _context.Roles.Where(r => selectedIds.Contains(r.ID)).ToListAsync();
                person.Roles = roles;
            }
            else
            {
                person.Roles = new List<Role>();
            }

            // determina IsClient se qualquer papel selecionado for "Cliente"
            if (person.Roles.Any(r => string.Equals(r.Name, "Cliente", StringComparison.OrdinalIgnoreCase)))
            {
                // armazenar apenas dígitos como string (evita problemas com tipos numéricos)
                var cpfRaw = Regex.Replace(Request.Form["CPF"].ToString() ?? "", @"\D", "");
                var cpf = string.IsNullOrWhiteSpace(cpfRaw) ? null : cpfRaw;

                var phoneRaw = Regex.Replace(Request.Form["PhoneNumber"].ToString() ?? "", @"\D", "");
                var phone = string.IsNullOrWhiteSpace(phoneRaw) ? null : phoneRaw;

                var email = Request.Form["Email"].ToString();
                person.MakeClient(cpf, string.IsNullOrWhiteSpace(email) ? null : email, phone);
            }
            else
            {
                person.IsClient = false;
                person.CPF = null;
                person.Email = null;
                person.PhoneNumber = null;
            }

            // Mapear outras flags booleanas a partir das Roles selecionadas
            person.IsActor = person.Roles.Any(r => string.Equals(r.Name, "Ator", StringComparison.OrdinalIgnoreCase));
            person.IsDirector = person.Roles.Any(r => string.Equals(r.Name, "Diretor", StringComparison.OrdinalIgnoreCase));
            person.IsStudent = person.Roles.Any(r => string.Equals(r.Name, "Estudante", StringComparison.OrdinalIgnoreCase));

            await _personRepository.Create(person);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _personRepository.GetById(id);
            if (person == null) return NotFound();
            await _personRepository.Delete(person);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            // incluir Roles para que possamos marcar os checkboxes
            var model = await _context.Persons
                                      .Include(p => p.Roles)
                                      .FirstOrDefaultAsync(p => p.ID == id);

            if (model == null) return NotFound();

            var selectedIds = model.Roles?.Select(r => r.ID) ?? Enumerable.Empty<int>();
            await PopulateRolesAsync(selectedIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(Person person)
        {
            // parse selected roles ids (checkboxes name = selectedRoles)
            var selected = Request.Form["selectedRoles"].ToArray();
            var selectedIds = selected.Select(s => int.TryParse(s, out var id) ? id : (int?)null)
                                      .Where(id => id.HasValue)
                                      .Select(id => id!.Value)
                                      .ToList();

            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync(selectedIds);
                return View(person);
            }

            // carregue a entidade existente com Roles para manipular a coleção rastreada
            var existing = await _context.Persons
                                         .Include(p => p.Roles)
                                         .FirstOrDefaultAsync(p => p.ID == person.ID);

            if (existing == null) return NotFound();

            // atualizar campos escalares
            existing.FirstName = person.FirstName;
            existing.LastName = person.LastName;
            existing.BirthDate = person.BirthDate;
            existing.EnrollmentDate = person.EnrollmentDate;

            // carregar as roles selecionadas (entidades rastreadas)
            var selectedRoles = selectedIds.Any()
                ? await _context.Roles.Where(r => selectedIds.Contains(r.ID)).ToListAsync()
                : new List<Role>();

            // remover roles que foram desmarcadas
            var toRemove = existing.Roles.Where(r => !selectedIds.Contains(r.ID)).ToList();
            foreach (var r in toRemove)
            {
                existing.Roles.Remove(r);
            }

            // adicionar roles novas selecionadas
            var existingRoleIds = existing.Roles.Select(r => r.ID).ToHashSet();
            foreach (var r in selectedRoles.Where(r => !existingRoleIds.Contains(r.ID)))
            {
                existing.Roles.Add(r);
            }

            // determina IsClient se qualquer papel selecionado for "Cliente"
            if (existing.Roles.Any(r => string.Equals(r.Name, "Cliente", StringComparison.OrdinalIgnoreCase)))
            {
                var cpfRaw = Regex.Replace(Request.Form["CPF"].ToString() ?? "", @"\D", "");
                var cpf = string.IsNullOrWhiteSpace(cpfRaw) ? null : cpfRaw;

                var phoneRaw = Regex.Replace(Request.Form["PhoneNumber"].ToString() ?? "", @"\D", "");
                var phone = string.IsNullOrWhiteSpace(phoneRaw) ? null : phoneRaw;

                var email = Request.Form["Email"].ToString();
                existing.MakeClient(cpf, string.IsNullOrWhiteSpace(email) ? null : email, phone);
            }
            else
            {
                existing.IsClient = false;
                existing.CPF = null;
                existing.Email = null;
                existing.PhoneNumber = null;
            }

            // Mapear outras flags booleanas a partir das Roles selecionadas
            existing.IsActor = existing.Roles.Any(r => string.Equals(r.Name, "Ator", StringComparison.OrdinalIgnoreCase));
            existing.IsDirector = existing.Roles.Any(r => string.Equals(r.Name, "Diretor", StringComparison.OrdinalIgnoreCase));
            existing.IsStudent = existing.Roles.Any(r => string.Equals(r.Name, "Estudante", StringComparison.OrdinalIgnoreCase));

            // salvar alterações (a entidade 'existing' já está rastreada)
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }
            ViewBag.SearchQuery = query;
            var persons = await _personRepository.GetByName(query);
            return View("Index", persons);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}