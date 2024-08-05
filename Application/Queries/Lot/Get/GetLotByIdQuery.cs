using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Lot.Get
{
    public class GetLotByIdQuery : IRequest<Core.Entities.Lot>
    {
        public int Id { get; set; }
    }
}
