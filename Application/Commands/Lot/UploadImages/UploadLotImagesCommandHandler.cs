using Application.Exceptions;
using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Lot.UploadImages
{
    public class UploadLotImagesCommandHandler : IRequestHandler<UploadLotImagesCommand, List<string>>
    {
        private readonly AppDbContext _context;
        private readonly ICloudflareR2Service _r2Service;
        private readonly ILogger<UploadLotImagesCommandHandler> _logger;

        public UploadLotImagesCommandHandler(
            AppDbContext context,
            ICloudflareR2Service r2Service,
            ILogger<UploadLotImagesCommandHandler> logger)
        {
            _context = context;
            _r2Service = r2Service;
            _logger = logger;
        }

        public async Task<List<string>> Handle(UploadLotImagesCommand request, CancellationToken cancellationToken)
        {
           
            var lot = await _context.Lots
                .FirstOrDefaultAsync(l => l.Id == request.LotId, cancellationToken);

            if (lot == null)
            {
                throw new NotFoundException(nameof(Lot), request.LotId);
            }

            var uploadedUrls = new List<string>();
            var uploadedImages = new List<LotImage>();

            try
            {
               
                for (int i = 0; i < request.ImageFiles.Count; i++)
                {
                    var file = request.ImageFiles[i];
                    var isMain = request.IsMain && i == 0;

                    try
                    {
                        using var stream = file.OpenReadStream();
                        var imageUrl = await _r2Service.UploadImageAsync(stream, file.FileName, file.ContentType);

                        uploadedUrls.Add(imageUrl);

                       
                        var lotImage = new LotImage
                        {
                            LotId = request.LotId,
                            FileName = file.FileName,
                            FileUrl = imageUrl,
                            ContentType = file.ContentType,
                            FileSize = file.Length,
                            IsMain = isMain,
                            CreatedAt = DateTime.UtcNow
                        };

                        uploadedImages.Add(lotImage);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError(fileEx, "Ошибка при загрузке файла {FileName} для лота {LotId}", file.FileName, request.LotId);
                        
                       
                        if (i == 0 && request.ImageFiles.Count > 1)
                        {
                            _logger.LogWarning("Первый файл {FileName} не загрузился, продолжаем с остальными", file.FileName);
                            continue;
                        }
                        
                       
                        if (uploadedUrls.Count > 0)
                        {
                            _logger.LogWarning("Загружено {UploadedCount} из {TotalCount} файлов", uploadedUrls.Count, request.ImageFiles.Count);
                            break;
                        }
                        
                       
                        throw new BusinessException($"Ошибка при загрузке файла '{file.FileName}': {fileEx.Message}");
                    }
                }

               
                if (request.IsMain && uploadedImages.Any())
                {
                    var existingImages = await _context.LotImages
                        .Where(img => img.LotId == request.LotId)
                        .ToListAsync(cancellationToken);

                    foreach (var existingImage in existingImages)
                    {
                        existingImage.IsMain = false;
                    }
                }

                
                _context.LotImages.AddRange(uploadedImages);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Успешно загружено {Count} изображений для лота {LotId}", 
                    uploadedImages.Count, request.LotId);

                return uploadedUrls;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при загрузке изображений для лота {LotId}", request.LotId);
                
                
                foreach (var url in uploadedUrls)
                {
                    try
                    {
                        await _r2Service.DeleteImageAsync(url);
                    }
                    catch (Exception deleteEx)
                    {
                        _logger.LogWarning(deleteEx, "Не удалось удалить файл {Url} после ошибки загрузки", url);
                    }
                }

                throw new BusinessException($"Ошибка при загрузке изображений: {ex.Message}");
            }
        }
    }
}
