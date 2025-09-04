using FluentValidation;

namespace Application.Commands.Bid.PlaceBid
{
    public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidCommandValidator()
        {
            RuleFor(x => x.LotId)
                .GreaterThan(0)
                .WithMessage("LotId должен быть больше 0");

            // Валидацию суммы выполняем в обработчике (с учетом текущей цены и правил аукциона)
        }
    }
}