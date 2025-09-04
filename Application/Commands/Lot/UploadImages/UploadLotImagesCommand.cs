using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Lot.UploadImages
{
    public class UploadLotImagesCommand : IRequest<List<string>>
    {
        public int LotId { get; set; }
        public List<IFormFile> ImageFiles { get; set; } = new();
        public bool IsMain { get; set; } = false;
    }
}
