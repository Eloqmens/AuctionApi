using Core.Entities;
using MediatR;

namespace Application.Queries.Bid
{
    public class GetBidsQuery : IRequest<List<Core.Entities.Bid>>
    {
        public int LotId { get; set; }
    }
}
