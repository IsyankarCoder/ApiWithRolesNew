using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace ApiWithRolesFromScratch.Controllers
{
    [Authorize(Roles = "User")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController() { }

        public UserController(string name) { }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("You have access the User Controller");
        }
    }
}
