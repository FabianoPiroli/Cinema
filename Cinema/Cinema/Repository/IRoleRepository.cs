using Cinema.Models;

namespace Cinema.Repository
{
    public interface IRoleRepository
    {
        public Task Create(Role role);
        public Task Update(Role role);
        public Task Delete(Role role);
    }
}
