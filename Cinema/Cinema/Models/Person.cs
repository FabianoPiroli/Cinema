using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Person
    {
        [Key]
        public int ID { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public Role? role { get; set; }
    }
    public class Client : Person
    {
        public int? CPF { get; set; } = null;
        public string? Email { get; set; }
        public int? PhoneNumber { get; set; } = null;
        public bool IsStudent { get; set; } = false;
    }
}
