using FluentValidation;

namespace Application.Commands.Lot.UploadImages
{
    public class UploadLotImagesCommandValidator : AbstractValidator<UploadLotImagesCommand>
    {
        public UploadLotImagesCommandValidator()
        {
            RuleFor(x => x.LotId)
                .GreaterThan(0)
                .WithMessage("ID лота должен быть больше 0");

            RuleFor(x => x.ImageFiles)
                .NotEmpty()
                .WithMessage("Необходимо выбрать хотя бы одно изображение");

            RuleForEach(x => x.ImageFiles)
                .Must(file => file.Length > 0)
                .WithMessage("Файл не может быть пустым")
                .Must(file => file.Length <= 10 * 1024 * 1024) // 10MB
                .WithMessage("Размер файла не должен превышать 10MB")
                .Must(file => IsValidImageType(file.ContentType))
                .WithMessage("Поддерживаются только изображения в форматах JPEG, PNG, GIF, WebP");
        }

        private static bool IsValidImageType(string contentType)
        {
            var validTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            return validTypes.Contains(contentType.ToLower());
        }
    }
}
