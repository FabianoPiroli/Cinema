using Cinema.Models;
using Cinema.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.Repository
{
    public class RoomRepository : IRoomRepository
    {
         private readonly CinemaContext? _context;
        public RoomRepository(CinemaContext? context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAll()
        {
            var data = await _context.Rooms.ToListAsync();
            return data;
        }

        public async Task Create(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
        }
        public async Task Delete(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        public async Task Update(Room room)
        {
            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
        }
    }
}
