using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Cinema.Models
{
    public class Ticket
    {
        [Key]
        public int ID { get; set; }

        public float? Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        
        public Session? Session { get; set; }
        public Client? Client { get; set; }
        // if client is student chama StudentPrice
        public Ticket() {
            if (Client != null && Client.IsStudent)
            {
                Price = StudentPrice(Price ?? 0);
            }
        }
        public float StudentPrice(float basePrice)
        {
            return basePrice * 0.5f; // 50% discount for students
        }
    }
}
