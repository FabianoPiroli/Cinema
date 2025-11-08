namespace Cinema.Models
{
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;

        // Navegação inversa para suportar many-to-many entre Person e Role
        public ICollection<Person> Persons { get; set; } = new List<Person>();
    }
}