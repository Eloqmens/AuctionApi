using FluentValidation;

namespace Application.Commands.Lot.Update
{
    public class UpdateLotCommandValidator : AbstractValidator<UpdateLotCommand>
    {
        public UpdateLotCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id лота обязателен и должен быть больше 0");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Название лота обязательно")
                .MaximumLength(200)
                .WithMessage("Название лота не должно превышать 200 символов");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Описание лота обязательно")
                .MaximumLength(2000)
                .WithMessage("Описание лота не должно превышать 2000 символов");

            RuleFor(x => x.StartingPrice)
                .GreaterThan(0)
                .WithMessage("Начальная цена должна быть больше 0");

            RuleFor(x => x.EndTime)
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Время окончания аукциона должно быть в будущем");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .WithMessage("Категория обязательна");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("UserId обязателен");
        }
    }
}
