using Application.Models;
using MediatR;

namespace Application.Queries.Lot.GetAll
{
    public class GetLotsQuery : IRequest<PagedResult<Core.Entities.Lot>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
