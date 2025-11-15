using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema.Models
{
    public class Ticket
    {
        [Key]
        public int ID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [DataType(DataType.Currency)]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }

        public DateTime PurchaseDate { get; set; }
        
        public int SessionID { get; set; }

        [ForeignKey(nameof(SessionID))]
        public Session? Session { get; set; }
   
        public int? PersonID { get; set; }
        [ForeignKey(nameof(PersonID))]
        public Person? Person { get; set; }
        
        public decimal StudentPrice(decimal basePrice)
        {
            return basePrice * 0.5m; // 50% discount for students
        }
        
        public bool IsAllowedEntry(int? ageRating)
        {
            if (ageRating == null)
                return true;
            if (Person == null)
                return false;
            var clientAge = Person.GetAge();
            return clientAge >= ageRating;
        }
    }
}