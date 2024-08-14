using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.User.Logout
{
    public class LogoutCommand : IRequest
    {
        public string UserId { get; set; }
    }
}
