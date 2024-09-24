using KironTest.Entities;

namespace KironTest.Repositories.CoinRepositories
{
    public interface ICoinRepository
    {
        Task<Coin?> GetCoinByIdAsync(int id);
        Task<Coin?> GetCoinByNameAsync(string name);
        Task<IEnumerable<Coin>> GetAllCoinsAsync();
        Task FetchAndStoreCoinsAsync();
    }
}
