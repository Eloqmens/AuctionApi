using FluentValidation;

namespace Application.Commands.Lot.Create
{
    public class CreateLotCommandValidator : AbstractValidator<CreateLotCommand>
    {
        public CreateLotCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.StartingPrice).GreaterThan(0).WithMessage("Starting price must be greater than 0.");
            RuleFor(x => x.EndTime).GreaterThan(DateTime.Now).WithMessage("End time must be in the future.");
            RuleFor(x => x.CategoryId).GreaterThan(0).WithMessage("Category ID is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }
}
