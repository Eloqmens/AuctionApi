using MediatR;

namespace Application.Commands.Lot.DeleteImage
{
    public class DeleteLotImageCommand : IRequest
    {
        public int ImageId { get; set; }
    }
}
