using MediatR;

namespace Application.Commands.Lot.Delete
{
    public class DeleteLotCommand : IRequest
    {
        public int Id { get; set; }
    }
}
