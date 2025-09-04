using Application.Exceptions;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Lot.DeleteImage
{
    public class DeleteLotImageCommandHandler : IRequestHandler<DeleteLotImageCommand>
    {
        private readonly AppDbContext _context;
        private readonly ICloudflareR2Service _r2Service;
        private readonly ICurrentUserService _currentUserService;

        public DeleteLotImageCommandHandler(
            AppDbContext context, 
            ICloudflareR2Service r2Service,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _r2Service = r2Service;
            _currentUserService = currentUserService;
        }

        public async Task Handle(DeleteLotImageCommand request, CancellationToken cancellationToken)
        {
           
            var lotImage = await _context.LotImages
                .FirstOrDefaultAsync(i => i.Id == request.ImageId, cancellationToken);

            if (lotImage == null)
            {
                throw new NotFoundException(nameof(LotImage), request.ImageId);
            }

            try
            {
                await _r2Service.DeleteImageAsync(lotImage.FileUrl);

               
                _context.LotImages.Remove(lotImage);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Ошибка при удалении изображения: {ex.Message}");
            }
        }
    }
}
