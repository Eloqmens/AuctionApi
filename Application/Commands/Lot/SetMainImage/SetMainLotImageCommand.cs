using MediatR;

namespace Application.Commands.Lot.SetMainImage
{
    public class SetMainLotImageCommand : IRequest<Unit>
    {
        public int LotId { get; set; }
        public int ImageId { get; set; }
    }
}


