using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    public class BaseController : Controller
    {
        protected string UserId
        {
            get
            {
                if (HttpContext == null || HttpContext.User == null)
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return string.Empty;
                }

                var userId = HttpContext.User.FindFirst("sub")?.Value
                             ?? HttpContext.User.FindFirst("jti")?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    Console.WriteLine("User identifier (sub or jti) is null or empty.");
                    return string.Empty;
                }

                return userId;
            }
        }
    }
}
