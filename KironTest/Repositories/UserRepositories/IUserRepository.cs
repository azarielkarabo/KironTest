using KironTest.Entities;

namespace KironTest.Repositories.UserRepositories
{
    public interface IUserRepository
    {
        Task<User?> GetUserByUsername(string username);
        Task AddUser(User user);
    }
}
