using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Person
    {
        [Key]
        public int ID { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        // Alterado para DateOnly para armazenar apenas a data (sem hora)
        public DateOnly BirthDate { get; set; }

        // Alterado para DateOnly para armazenar apenas a data (sem hora)
        public DateOnly EnrollmentDate { get; set; }

        public Role? role { get; set; }

        public float GetAge()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - BirthDate.Year;
            if (today < BirthDate.AddYears(age))
                age--;
            return age;
        }
    }
    public class Client : Person
    {
        public int? CPF { get; set; } = null;
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; } = null;
        public bool IsStudent { get; set; } = false;
    }
}
