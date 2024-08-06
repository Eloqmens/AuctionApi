using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Bid.PlaceBid
{
    public class PlaceBidCommand : IRequest<Unit>
    {
        public int LotId { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
    }
}
