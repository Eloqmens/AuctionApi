using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Infrastructure.Identity;

namespace Application.Queries.User
{
    public class GetUserByIdQueryHandler
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public GetUserByIdQueryHandler(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }


        
    }
}
