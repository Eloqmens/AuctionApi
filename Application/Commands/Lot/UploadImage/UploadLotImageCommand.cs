using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Lot.UploadImage
{
    public class UploadLotImageCommand : IRequest<string>
    {
        public int LotId { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
        public bool IsMain { get; set; } = false;
    }
}
