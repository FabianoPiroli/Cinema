using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Person
    {
        [Key]
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateOnly? EnrollmentDate { get; set; }

        public bool IsClient { get; set; } = false;
        public bool IsActor { get; set; } = false;
        public bool IsDirector { get; set; } = false;
        public bool IsStudent { get; set; } = false;

        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }

        public ICollection<Role> Roles { get; set; } = new List<Role>();
        public ICollection<Movie> ActedMovies { get; set; } = new List<Movie>();
        public ICollection<Movie> DirectedMovies { get; set; } = new List<Movie>();

        public void MakeClient(string? cpf = null, string? email = null, string? phoneNumber = null)
        {
            IsClient = true;
            CPF = cpf;
            Email = email;
            PhoneNumber = phoneNumber;
        }

        public float GetAge()
        {
            if (BirthDate == null) return 0;
            var today = DateTime.Today;
            var birth = new DateTime(BirthDate.Value.Year, BirthDate.Value.Month, BirthDate.Value.Day);
            var age = today.Year - birth.Year;
            if (birth > today.AddYears(-age)) age--;
            return age;
        }
    }
}