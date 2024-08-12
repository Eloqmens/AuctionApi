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
            return await _context.Lots.Include(l => l.Category)
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
        }
    }
}
