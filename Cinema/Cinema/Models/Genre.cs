using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Genre
    {
        [Key]
        public int ID { get; set; }
        public string? Name { get; set; }
    }
}
