using Cinema.Models;

namespace Cinema.Repository
{
    public interface IPersonRepository
    {
        public Task Create(Person person);
        public Task Update(Person person);
        public Task Delete(Person person);
        public Task GetAll();
    }
}
