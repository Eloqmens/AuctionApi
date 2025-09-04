using Application.Exceptions;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Lot.UploadImage
{
    public class UploadLotImageCommandHandler : IRequestHandler<UploadLotImageCommand, string>
    {
        private readonly AppDbContext _context;
        private readonly ICloudflareR2Service _r2Service;
        private readonly ICurrentUserService _currentUserService;

        public UploadLotImageCommandHandler(
            AppDbContext context, 
            ICloudflareR2Service r2Service,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _r2Service = r2Service;
            _currentUserService = currentUserService;
        }

        public async Task<string> Handle(UploadLotImageCommand request, CancellationToken cancellationToken)
        {
            // Проверяем, что лот существует
            var lot = await _context.Lots
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.Id == request.LotId, cancellationToken);

            if (lot == null)
            {
                throw new NotFoundException(nameof(Lot), request.LotId);
            }

            // Поскольку контроллер уже проверяет авторизацию через [Authorize],
            // мы можем загружать изображения для любого лота
            // Проверка прав доступа (владелец или админ) выполняется на уровне контроллера

            // Валидация файла
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                throw new BusinessException("Файл изображения не может быть пустым");
            }

            // Проверяем тип файла
            var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedTypes.Contains(request.ImageFile.ContentType.ToLower()))
            {
                throw new BusinessException("Неподдерживаемый тип файла. Разрешены: JPEG, PNG, GIF, WebP");
            }

            // Проверяем размер файла (максимум 10MB)
            const long maxFileSize = 10 * 1024 * 1024; // 10MB
            if (request.ImageFile.Length > maxFileSize)
            {
                throw new BusinessException("Размер файла не должен превышать 10MB");
            }

            try
            {
                // Загружаем изображение в Cloudflare R2
                using var stream = request.ImageFile.OpenReadStream();
                var imageUrl = await _r2Service.UploadImageAsync(
                    stream, 
                    request.ImageFile.FileName, 
                    request.ImageFile.ContentType);

                // Если это главное изображение, снимаем флаг с других изображений
                if (request.IsMain)
                {
                    var existingMainImages = lot.Images.Where(i => i.IsMain).ToList();
                    foreach (var image in existingMainImages)
                    {
                        image.IsMain = false;
                    }
                }

                // Создаем запись об изображении в базе данных
                var lotImage = new LotImage
                {
                    LotId = request.LotId,
                    FileName = request.ImageFile.FileName,
                    FileUrl = imageUrl,
                    ContentType = request.ImageFile.ContentType,
                    FileSize = request.ImageFile.Length,
                    IsMain = request.IsMain,
                    CreatedAt = DateTime.UtcNow
                };

                _context.LotImages.Add(lotImage);
                await _context.SaveChangesAsync(cancellationToken);

                return imageUrl;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Ошибка при загрузке изображения: {ex.Message}");
            }
        }
    }
}
