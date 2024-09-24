using KironTest.Entities;
using KironTest.Models;
using KironTest.Services.Cache;
using KironTest.Services.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KironTest.Repositories.BankHolidayRepositories
{
    public class BankHolidayRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BankHolidayRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService _cacheService;

        public BankHolidayRepository(ApplicationDbContext context,
                                     ILogger<BankHolidayRepository> logger,
                                     IHttpClientFactory httpClientFactory,
                                     ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
        }

        public async Task<Holiday?> GetHolidayByIdAsync(int id)
        {
            string cacheKey = $"Holiday_{id}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Holidays.FindAsync(id), TimeSpan.FromMinutes(30));
        }

        public async Task<Holiday?> GetHolidayByTitleAndDateAsync(string title, DateTime date)
        {
            string cacheKey = $"Holiday_{title}_{date:yyyyMMdd}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
                await _context.Holidays.FirstOrDefaultAsync(i => i.Title == title && i.Date == date), TimeSpan.FromMinutes(30));
        }

        public async Task<IEnumerable<Holiday>> GetAllHolidaysAsync()
        {
            string cacheKey = "AllHolidays";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Holidays.ToListAsync(), TimeSpan.FromMinutes(30));
        }

        public async Task AddHolidayAsync(Holiday holiday)
        {
            _context.Holidays.Add(holiday);
            await _context.SaveChangesAsync();
            _cacheService.Remove($"Holiday_{holiday.Id}");
            _cacheService.Remove("AllHolidays");
        }

        public async Task<RegionHoliday?> GetRegionHolidayByIdAsync(int id)
        {
            return await _context.RegionHolidays.FindAsync(id);
        }

        public async Task<IEnumerable<RegionHoliday>> GetAllRegionHolidaysAsync()
        {
            return await _context.RegionHolidays.ToListAsync();
        }

        public async Task<bool> RegionHolidayExists(Region region, Holiday holiday)
        {
            return await _context.RegionHolidays.AnyAsync(i => i.Region == region && i.Holiday == holiday);
        }

        public async Task AddRegionHolidayAsync(Region region, Holiday holiday)
        {
            if (!await RegionHolidayExists(region, holiday))
            {
                _context.RegionHolidays.Add(new RegionHoliday { Holiday = holiday, Region = region });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Region?> GetRegionByIdAsync(int id)
        {
            string cacheKey = $"Region_{id}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Regions.FindAsync(id), TimeSpan.FromMinutes(30));
        }

        public async Task<Region?> GetRegionByNameAsync(string name)
        {
            string cacheKey = $"Region_{name}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Regions.FirstOrDefaultAsync(i => i.Name == name), TimeSpan.FromMinutes(30));
        }

        public async Task<IEnumerable<Region>> GetAllRegionsAsync()
        {
            string cacheKey = "AllRegions";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Regions.ToListAsync(), TimeSpan.FromMinutes(30));
        }

        public async Task AddRegionAsync(Region region)
        {
            _context.Regions.Add(region);
            await _context.SaveChangesAsync();
            _cacheService.Remove($"Region_{region.Id}");
            _cacheService.Remove("AllRegions");
        }

        public async Task FetchAndStoreBankHolidaysAsync()
        {
            var client = _httpClientFactory.CreateClient();
            string jsonResponse;

            try
            {
                jsonResponse = await client.GetStringAsync("https://www.gov.uk/bank-holidays.json");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching bank holidays: {ex.Message}");
                throw new Exception("Bank holiday API is down!", ex);
            }

            var bankHolidays = JsonConvert.DeserializeObject<BankHolidayModel>(jsonResponse);

            if (bankHolidays == null)
            {
                throw new Exception("Failed to deserialize bank holidays.");
            }

            foreach (var region in bankHolidays.Regions)
            {
                var regionEntity = await GetRegionByNameAsync(region.Division)
                    ?? new Region { Name = region.Division };

                if (regionEntity.Id == 0)
                {
                    await AddRegionAsync(regionEntity);
                }

                foreach (var holiday in region.Events)
                {
                    var date = DateTime.Parse(holiday.Date);
                    var holidayEntity = await GetHolidayByTitleAndDateAsync(holiday.Title, date)
                        ?? new Holiday
                        {
                            Title = holiday.Title,
                            Date = date,
                            Notes = holiday.Notes,
                            Bunting = bool.Parse(holiday.Bunting)
                        };

                    if (holidayEntity.Id == 0)
                    {
                        await AddHolidayAsync(holidayEntity);
                    }

                    await AddRegionHolidayAsync(regionEntity, holidayEntity);
                }
            }
        }

        public async Task<RegionModel> GetHolidaysByRegionAsync(string region)
        {
            string cacheKey = $"HolidaysByRegion_{region}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var regionEntity = await GetRegionByNameAsync(region)
                    ?? throw new Exception("Region not found");

                var holidays = await _context.RegionHolidays
                    .Where(rh => rh.RegionId == regionEntity.Id)
                    .Include(rh => rh.Holiday)
                    .Select(rh => new HolidayModel
                    {
                        Bunting = rh.Holiday.Bunting.ToString(),
                        Date = rh.Holiday.Date.ToString("yyyy-MM-dd"),
                        Notes = rh.Holiday.Notes,
                        Title = rh.Holiday.Title
                    })
                    .ToListAsync();

                return new RegionModel
                {
                    Division = region,
                    Events = holidays
                };
            }, TimeSpan.FromMinutes(30));
        }
    }
}
