using ApiWithRolesFromScratch.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiWithRolesFromScratch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController
        : ControllerBase
    {

        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IConfiguration _config;

        public AccountController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var user = new IdentityUser { UserName = model.Username };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { message = "User registered Successfully" });
            }
            return BadRequest(result);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.Username);
            if (user is not null && await _userManager.CheckPasswordAsync(user!, loginModel.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                     new Claim(JwtRegisteredClaimNames.Sub,user.UserName!),
                     new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                };

                var selectedRole = userRoles.Select(role => new Claim(ClaimTypes.Role, role));
                authClaims.AddRange(selectedRole);

                var token = new JwtSecurityToken(
                    issuer: _config["Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_config["Jwt:ExpiryMinutes"]!)),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(
                                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)),
                                            SecurityAlgorithms.HmacSha256)

                    );

                var responseToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(responseToken);

            }

            return Unauthorized();
        }


        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole([FromBody] string Role)
        {
            if (!await _roleManager.RoleExistsAsync(Role))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(Role));
                if (result.Succeeded)
                {
                    return Ok(new { message = "Role Added Successfully" });
                }
                return BadRequest(result.Errors);
            }
            return BadRequest("Role Already Exists");

        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] UserRole userRole)
        {
            var user = await _userManager.FindByNameAsync(userRole.UserName);

            if (user is null)
            {
                return BadRequest("User not found");

            }

            var result = await _userManager.AddToRoleAsync(user, userRole.Role);
            if (result.Succeeded)
            {
                return Ok(new { message = "Role assigned successfully" });
            }

            return BadRequest(result.Errors);

        }

    }
}
