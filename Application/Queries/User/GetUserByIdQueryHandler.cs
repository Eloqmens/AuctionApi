using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserByIdQueryHandler
    {
        private readonly AppDbContext _appDbContext;
        public GetUserByIdQueryHandler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        //public async Task<Core.Entities.User> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        //{
        //    var user = await _appDbContext.Users.FindAsync(request.Id);
        //    if (user == null)
        //    {
        //        return null;
        //    }

        //    return new Core.Entities.User
        //    {
        //        Username = user.Username,
        //    };
        //}
    }
}
