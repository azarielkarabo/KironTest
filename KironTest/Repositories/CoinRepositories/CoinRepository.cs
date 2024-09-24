using KironTest.Entities;
using KironTest.Models;
using KironTest.Services.Cache;
using KironTest.Services.Database;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace KironTest.Repositories.CoinRepositories
{
    public class CoinRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CoinRepository> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICacheService _cacheService;

        public CoinRepository(ApplicationDbContext context,
                              ILogger<CoinRepository> logger,
                              IHttpClientFactory httpClientFactory,
                              ICacheService cacheService)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _cacheService = cacheService;
        }

        public async Task<Coin?> GetCoinByIdAsync(int id)
        {
            string cacheKey = $"Coin_{id}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Coins.FindAsync(id), TimeSpan.FromMinutes(30));
        }

        public async Task<Coin?> GetCoinByNameAsync(string name)
        {
            string cacheKey = $"Coin_{name}";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            await _context.Coins.FirstOrDefaultAsync(i => i.Name == name), TimeSpan.FromMinutes(30));
        }

        public async Task<IEnumerable<Coin>> GetAllCoinsAsync()
        {
            string cacheKey = "AllCoins";
            return await _cacheService.GetOrCreateAsync(cacheKey, async () => await _context.Coins.ToListAsync(), TimeSpan.FromMinutes(60));
        }

        public async Task FetchAndStoreCoinsAsync()
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://openapiv1.coinstats.app/coins");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Error fetching coins. Status Code: {response.StatusCode}, Response: {errorContent}");
                throw new Exception($"Error fetching coins. Status Code: {response.StatusCode}, Response: {errorContent}");
            }

            string jsonResponse = await response.Content.ReadAsStringAsync();

            var coinsResponse = JsonConvert.DeserializeObject<CoinResponse>(jsonResponse);

            if (coinsResponse?.Result == null)
            {
                throw new Exception("Failed to deserialize coins.");
            }

            foreach (var coinModel in coinsResponse.Result)
            {
                var coinEntity = await GetCoinByNameAsync(coinModel.Name);

                if (coinEntity == null)
                {
                    coinEntity = new Coin
                    {
                        Id = coinModel.Id,
                        Icon = coinModel.Icon,
                        Name = coinModel.Name,
                        Symbol = coinModel.Symbol,
                        Rank = coinModel.Rank,
                        Price = coinModel.Price,
                        PriceBtc = coinModel.PriceBtc,
                        Volume = coinModel.Volume,
                        MarketCap = coinModel.MarketCap,
                        AvailableSupply = coinModel.AvailableSupply,
                        TotalSupply = coinModel.TotalSupply,
                        FullyDilutedValuation = coinModel.FullyDilutedValuation,
                        PriceChange1h = coinModel.PriceChange1h,
                        PriceChange1d = coinModel.PriceChange1d,
                        PriceChange1w = coinModel.PriceChange1w,
                        RedditUrl = coinModel.RedditUrl,
                        TwitterUrl = coinModel.TwitterUrl,
                        Explorers = coinModel.Explorers,
                        WebsiteUrl = coinModel.WebsiteUrl,
                        ContractAddress = coinModel.ContractAddress,
                        Decimals = coinModel.Decimals
                    };

                    _context.Coins.Add(coinEntity);
                }
                else
                {
                    // Update existing coin entity with new properties
                    coinEntity.Icon = coinModel.Icon;
                    coinEntity.Symbol = coinModel.Symbol;
                    coinEntity.Rank = coinModel.Rank;
                    coinEntity.Price = coinModel.Price;
                    coinEntity.PriceBtc = coinModel.PriceBtc;
                    coinEntity.Volume = coinModel.Volume;
                    coinEntity.MarketCap = coinModel.MarketCap;
                    coinEntity.AvailableSupply = coinModel.AvailableSupply;
                    coinEntity.TotalSupply = coinModel.TotalSupply;
                    coinEntity.FullyDilutedValuation = coinModel.FullyDilutedValuation;
                    coinEntity.PriceChange1h = coinModel.PriceChange1h;
                    coinEntity.PriceChange1d = coinModel.PriceChange1d;
                    coinEntity.PriceChange1w = coinModel.PriceChange1w;
                    coinEntity.RedditUrl = coinModel.RedditUrl;
                    coinEntity.TwitterUrl = coinModel.TwitterUrl;
                    coinEntity.Explorers = coinModel.Explorers?.ToList();
                    coinEntity.WebsiteUrl = coinModel.WebsiteUrl;
                    coinEntity.ContractAddress = coinModel.ContractAddress;
                    coinEntity.Decimals = coinModel.Decimals;

                    _context.Coins.Update(coinEntity);
                }

                _cacheService.Remove($"Coin_{coinEntity.Id}");
                _cacheService.Remove("AllCoins");

                await _context.SaveChangesAsync();
            }
        }

    }
}
