using Cinema.Models;

namespace Cinema.Repository
{
    public interface IRoomRepository
    {
        public Task Create(Room room);
        public Task Update(Room room);
        public Task Delete(Room room);
    }
}
