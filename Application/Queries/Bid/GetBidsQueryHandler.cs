using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Bid
{
    public class GetBidsQueryHandler : IRequestHandler<GetBidsQuery, List<Core.Entities.Bid>>
    {
        private readonly AppDbContext _context;

        public GetBidsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Core.Entities.Bid>> Handle(GetBidsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Bids
                .Where(b => b.LotId == request.LotId)
                .ToListAsync(cancellationToken);
        }
    }
}
