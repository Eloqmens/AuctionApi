using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Lot.Get
{
    public class GetLotByIdQueryHandler : IRequestHandler<GetLotByIdQuery, Core.Entities.Lot>
    {
        private readonly AppDbContext _context;

        public GetLotByIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Core.Entities.Lot> Handle(GetLotByIdQuery request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots
                .Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

            if (lot != null)
            {
                lot.Images = await _context.LotImages
                    .Where(img => img.LotId == lot.Id)
                    .ToListAsync(cancellationToken);
            }

            return lot;
        }
    }
}
