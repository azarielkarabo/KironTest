using KironTest.Repositories.CoinRepositories;
using Microsoft.AspNetCore.Mvc;

namespace KironTest.Controllers
{
    [Route("api/[controller]")]
    public class CoinsController : ControllerBase
    {
        private readonly CoinRepository _coinRepository;
        
        public CoinsController(CoinRepository coinRepository)
        {
            _coinRepository = coinRepository;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchAndStoreCoinsAsync()
        {
            await _coinRepository.FetchAndStoreCoinsAsync();
            return Ok("Coins fetched and stored.");
        }

        [HttpGet("coins")]
        public async Task<IActionResult> GetAllCoinsAsync()
        {
            var coins = await _coinRepository.GetAllCoinsAsync();
            return Ok(coins);
        }

        [HttpGet("{coinName}")]
        public async Task<IActionResult> GetCoinByNameAsync(string coinName)
        {
            var coin = await _coinRepository.GetCoinByNameAsync(coinName);
            return Ok(coin);
        }
    }
}
