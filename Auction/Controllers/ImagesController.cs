using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ICloudflareR2Service _r2Service;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ICloudflareR2Service r2Service, ILogger<ImagesController> logger)
        {
            _r2Service = r2Service;
            _logger = logger;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> GetImage(string fileName)
        {
            try
            {
                var imageUrl = await _r2Service.GetImageUrlAsync(fileName);
                
                using var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(imageUrl);
                
                if (response.IsSuccessStatusCode)
                {
                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                    return File(imageBytes, contentType);
                }
                
                
                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Ошибка при загрузке изображения {FileName}: {StatusCode}", fileName, response.StatusCode);
                }
                
                return NotFound();
            }
            catch (Exception ex)
            {
                
                _logger.LogError(ex, "Ошибка при получении изображения {FileName}", fileName);
                return NotFound();
            }
        }
    }
}
