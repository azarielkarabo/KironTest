using Microsoft.AspNetCore.Authorization; 
using Microsoft.AspNetCore.Mvc;
using KironTest.Repositories.BankHolidayRepositories;

namespace KironTest.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize] 
    public class BankHolidaysController : ControllerBase
    {
        private readonly BankHolidayRepository _bankHolidayRepository;

        public BankHolidaysController(BankHolidayRepository bankHolidayRepository)
        {
            _bankHolidayRepository = bankHolidayRepository;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchBankHolidays()
        {
            // Check if the process is already completed
            // if (await _bankHolidayRepository.HasBeenFetchedAsync())
            // {
            //     return BadRequest("The work for this endpoint has been fulfilled.");
            // }

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
