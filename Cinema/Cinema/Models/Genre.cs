using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Cinema.Models
{
    public class Genre
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }

        // colecao many-to-many inversa
        public ICollection<Movie> Movies { get; set; } = new List<Movie>();
    }
}
