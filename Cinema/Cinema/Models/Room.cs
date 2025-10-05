using System.ComponentModel.DataAnnotations;

namespace Cinema.Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        public int RoomNumber { get; set; }
        public int SeatNumber { get; set; }
        public int Capacity { get; set; }
    }
}
