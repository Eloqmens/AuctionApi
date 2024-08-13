using Application.Exceptions;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserNameByIdQueryHandler : IRequestHandler<GetUserNameByIdQuery, string>
    {
        private readonly UserManager<AppUser> _userManager;

        public GetUserNameByIdQueryHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string> Handle(GetUserNameByIdQuery request, CancellationToken cancellationToken)
        {
            
            var user = await _userManager.FindByIdAsync(request.UserId);

            if (user == null)
            {
                throw new NotFoundException(nameof(AppUser), request.UserId);
            }

 
            return user.UserName;
        }
    }
}
