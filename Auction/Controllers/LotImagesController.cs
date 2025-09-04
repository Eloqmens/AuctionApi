using Application.Commands.Lot.DeleteImage;
using Application.Commands.Lot.UploadImage;
using Application.Commands.Lot.UploadImages;
using Application.Commands.Lot.SetMainImage;
using Auction.Controllers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LotImagesController : BaseController
    {
        private readonly IMediator _mediator;

        public LotImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{lotId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImage(int lotId, [FromForm] IFormFile imageFile, [FromForm] bool isMain = false)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest("Файл изображения не может быть пустым");
            }

            var command = new UploadLotImageCommand
            {
                LotId = lotId,
                ImageFile = imageFile,
                IsMain = isMain
            };

            try
            {
                var imageUrl = await _mediator.Send(command);
                return Ok(new { imageUrl, message = "Изображение успешно загружено" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{lotId}/set-main/{imageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetMainImage(int lotId, int imageId)
        {
            var command = new SetMainLotImageCommand { LotId = lotId, ImageId = imageId };
            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{lotId}/bulk")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadImages(int lotId, [FromForm] List<IFormFile> imageFiles, [FromForm] bool isMain = false)
        {
            if (imageFiles == null || !imageFiles.Any() || imageFiles.All(f => f.Length == 0))
            {
                return BadRequest("Необходимо выбрать хотя бы одно изображение");
            }

            var command = new UploadLotImagesCommand
            {
                LotId = lotId,
                ImageFiles = imageFiles,
                IsMain = isMain
            };

            try
            {
                var imageUrls = await _mediator.Send(command);
                return Ok(new { imageUrls, message = $"Успешно загружено {imageUrls.Count} изображений" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{imageId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var command = new DeleteLotImageCommand
            {
                ImageId = imageId
            };

            try
            {
                await _mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
