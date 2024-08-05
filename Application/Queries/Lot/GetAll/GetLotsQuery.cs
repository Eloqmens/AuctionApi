using MediatR;

namespace Application.Queries.Lot.GetAll
{
    public class GetLotsQuery : IRequest<List<Core.Entities.Lot>>
    {
    }
}
