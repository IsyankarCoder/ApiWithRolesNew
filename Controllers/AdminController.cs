using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiWithRolesFromScratch.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController
        : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have access the Admin Controller");
        }
    }
}
