using Cinema.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.Repository
{
    public interface IRoomRepository
    {
        public Task<List<Room>> GetAll();
        public Task Create(Room room);
        public Task Update(Room room);
        public Task Delete(Room room);
    }
}
