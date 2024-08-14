using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserNameByIdQuery : IRequest<string>
    {
        public string UserId { get; set; }
    }
}
