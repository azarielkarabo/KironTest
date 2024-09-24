
using KironTest.Repositories.NavigationRepositories;
using Microsoft.AspNetCore.Mvc;

namespace KironTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavigationController : ControllerBase
    {
        private readonly INavigationRepository _navigationRepository;

        public NavigationController(INavigationRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }

        [HttpGet("tree")]
        public async  Task<IActionResult> GetNavigationTreeAsync()
        {
            var tree = await _navigationRepository.GetNavigationTreeAsync();
            return Ok(tree);
        }
    }
}
