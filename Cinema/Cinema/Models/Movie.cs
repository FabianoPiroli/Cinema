using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Movie
    {
        [Key]
        public int ID { get; set; }
        public string? Title { get; set; }
        public int DurationInMinutes { get; set; }
        public List<Genre>? Genres { get; set; }
        public List<Person>? Actors { get; set; }
        public List<Person>? Directors { get; set; }
    }
}
