using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Movie
    {
        [Key]
        public int ID { get; set; }
        public string? Title { get; set; }
        public int DurationInMinutes { get; set; }
        public int? AgeRatingID { get; set; }
        [ForeignKey(nameof(AgeRatingID))]
        public AgeRating? AgeRating { get; set; }
        public string? ImageURL { get; set; }
        public string? Synopsis { get; set; }

        // coleções many-to-many
        public List<Genre> Genres { get; set; } = new();
        public List<Person> Actors { get; set; } = new();
        public List<Person> Directors { get; set; } = new();
    }
}