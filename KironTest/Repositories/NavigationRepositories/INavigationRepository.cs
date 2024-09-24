using KironTest.Models;

namespace KironTest.Repositories.NavigationRepositories
{
    public interface INavigationRepository
    {
        Task<List<NavigationModel>> GetNavigationTreeAsync();
    }
}
