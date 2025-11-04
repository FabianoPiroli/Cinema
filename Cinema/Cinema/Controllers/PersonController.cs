using Cinema.Models;
using Cinema.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.RegularExpressions;

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


        // Helper para popular a ViewBag com SelectList
        private async Task PopulateRolesAsync()
        {
            var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            // usamos Name como value e text para manter compatibilidade com código que lê o nome
            ViewBag.role = new SelectList(roles, "Name", "Name");
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
            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync(); // repopula quando houver erro de validação
                return View(person);
            }

            var roleValue = (Request.Form["role"].ToString() ?? "").Trim();

            // Se for cliente, crie uma instância Client e mapeie os campos adicionais
            if (string.Equals(roleValue, "Cliente", System.StringComparison.OrdinalIgnoreCase))
            {
                var client = new Client
                {
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    BirthDate = person.BirthDate,
                    EnrollmentDate = person.EnrollmentDate
                };

                // CPF e telefone enviados como strings formatadas -> remover não dígitos
                string cpfRaw = Regex.Replace(Request.Form["CPF"].ToString() ?? "", @"\D", "");
                if (int.TryParse(cpfRaw, out var cpfInt))
                    client.CPF = cpfInt;
                else
                    client.CPF = null;

                client.Email = Request.Form["Email"].ToString();

                string phoneRaw = Regex.Replace(Request.Form["PhoneNumber"].ToString() ?? "", @"\D", "");
                if (int.TryParse(phoneRaw, out var phoneInt))
                    client.PhoneNumber = phoneInt;
                else
                    client.PhoneNumber = null;

                // Tentar ligar role por nome (opcional)
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleValue);
                if (roleEntity != null) client.role = roleEntity;

                await _personRepository.Create(client);
            }
            else
            {
                // Não é cliente -> persistir Person normalmente, tentar vincular role por nome
                var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleValue);
                if (roleEntity != null) person.role = roleEntity;

                await _personRepository.Create(person);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var person = await _personRepository.GetById(id);
            if (person == null)
            {
                return NotFound();
            }
            await _personRepository.Delete(person);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var model = _context.Persons
                                .Include(p => p.role)
                                .FirstOrDefault(p => p.ID == id);

            if (model == null)
            {
                return NotFound();
            }

            ViewBag.role = new SelectList(_context.Roles.ToList(), "ID", "Name", model.role?.ID);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Person person)
        {
            if (ModelState.IsValid)
            {
                await _personRepository.Update(person);
                return RedirectToAction("Index");
            }
            return View(person);
        }

        public async Task<IActionResult> Search(string name)
        {
            var persons = await _personRepository.GetByName(name);
            return View("Index", persons);
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
