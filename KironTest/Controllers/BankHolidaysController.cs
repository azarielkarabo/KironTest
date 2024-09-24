using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using KironTest.Repositories.BankHolidayRepositories;

namespace KironTest.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BankHolidaysController : ControllerBase
    {
        private readonly IBankHolidayRepository _bankHolidayRepository;

        public BankHolidaysController(IBankHolidayRepository bankHolidayRepository)
        {
            _bankHolidayRepository = bankHolidayRepository;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchBankHolidays()
        {
            await _bankHolidayRepository.FetchAndStoreBankHolidaysAsync();
            return Ok("Bank holidays fetched and stored.");
        }

        [HttpGet("regions")]
        public async Task<IActionResult> GetRegions()
        {
            var regions = await _bankHolidayRepository.GetAllRegionsAsync();
            return Ok(regions);
        }

        [HttpGet("{region}")]
        public async Task<IActionResult> GetBankHolidays(string region)
        {
            var holidays = await _bankHolidayRepository.GetHolidaysByRegionAsync(region);
            return Ok(holidays);
        }
    }
}
