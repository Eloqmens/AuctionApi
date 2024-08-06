using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Queries.Category.GetAll
{
    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<Core.Entities.Category>>
    {
        private readonly AppDbContext _context;

        public GetCategoriesQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Core.Entities.Category>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Categories.ToListAsync(cancellationToken);
        }
    }
}
