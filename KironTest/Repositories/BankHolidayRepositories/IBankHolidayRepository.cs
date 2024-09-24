using KironTest.Entities;
using KironTest.Models;

namespace KironTest.Repositories.BankHolidayRepositories
{
    public interface IBankHolidayRepository
    {
        Task<Holiday?> GetHolidayByIdAsync(int id);
        Task<Holiday?> GetHolidayByTitleAndDateAsync(string title, DateTime date);
        Task<IEnumerable<Holiday>> GetAllHolidaysAsync();
        Task AddHolidayAsync(Holiday holiday);
        Task<RegionHoliday?> GetRegionHolidayByIdAsync(int id);
        Task<IEnumerable<RegionHoliday>> GetAllRegionHolidaysAsync();
        Task<bool> RegionHolidayExists(Region region, Holiday holiday);
        Task AddRegionHolidayAsync(Region region, Holiday holiday);
        Task<Region?> GetRegionByIdAsync(int id);
        Task<Region?> GetRegionByNameAsync(string name);
        Task<IEnumerable<Region>> GetAllRegionsAsync();
        Task AddRegionAsync(Region region);
        Task FetchAndStoreBankHolidaysAsync();
        Task<RegionModel> GetHolidaysByRegionAsync(string region);
    }
}
