using Application.Interfaces;
using System.Security.Claims;

namespace Auction.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var id = _httpContextAccessor.HttpContext?.User?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (Guid.TryParse(id, out var userId))
                {
                    return userId;
                }

                return Guid.Empty; 
            }
        }
    }

}
