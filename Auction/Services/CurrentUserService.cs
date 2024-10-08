﻿using Application.Interfaces;
using System.Security.Claims;

namespace Auction.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) =>
            _httpContextAccessor = httpContextAccessor;

        public string UserId
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null || _httpContextAccessor.HttpContext.User == null)
                {
                    Console.WriteLine("HttpContext or User is null.");
                    return string.Empty;
                }

                var id = _httpContextAccessor.HttpContext.User.FindFirst("sub")?.Value
                         ?? _httpContextAccessor.HttpContext.User.FindFirst("jti")?.Value;

                if (string.IsNullOrEmpty(id))
                {
                    Console.WriteLine("User identifier (sub or jti) is null or empty.");
                    return string.Empty;
                }

                return id;
            }
        }
    }

}
