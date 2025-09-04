using Application.Models;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Lot.GetAll
{
    public class GetLotsQueryHandler : IRequestHandler<GetLotsQuery, PagedResult<Core.Entities.Lot>>
    {
        private readonly AppDbContext _context;

        public GetLotsQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<Core.Entities.Lot>> Handle(GetLotsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Lots
                .Include(l => l.Category)
                .AsQueryable();

           
            if (request.CategoryId.HasValue)
            {
                query = query.Where(l => l.CategoryId == request.CategoryId.Value);
            }

            
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(l => l.Title.ToLower().Contains(searchTerm) || 
                                       l.Description.ToLower().Contains(searchTerm));
            }

           
            var totalCount = await query.CountAsync(cancellationToken);

            
            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            
            foreach (var lot in items)
            {
                lot.Images = await _context.LotImages
                    .Where(img => img.LotId == lot.Id)
                    .ToListAsync(cancellationToken);
            }

            return new PagedResult<Core.Entities.Lot>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }
}
