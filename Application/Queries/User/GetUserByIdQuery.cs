﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.User
{
    public class GetUserByIdQuery /*: IRequest<Core.Entities.User>*/
    {
        public int Id { get; set; }
    }
}
