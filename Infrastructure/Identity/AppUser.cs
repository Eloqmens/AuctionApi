using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public decimal Balance { get; set; }
        public string Wallet { get; set; }
    }
}
