using Auction.Models;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class AuthController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new AppUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);
                return Ok();
            }
            return BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
            if (result.Succeeded)
            {
                return Ok();
            }
            return Unauthorized();
        }
    }
}
