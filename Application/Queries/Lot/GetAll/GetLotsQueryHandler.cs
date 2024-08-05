using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Lot.GetAll
{
    public class GetLotsQueryHandler : IRequestHandler<GetLotsQuery, List<Core.Entities.Lot>>
    {
        private readonly AppDbContext _context;

        public GetLotsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Core.Entities.Lot>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Lots.Include(l => l.Category).ToListAsync(cancellationToken);
        }
    }
}
