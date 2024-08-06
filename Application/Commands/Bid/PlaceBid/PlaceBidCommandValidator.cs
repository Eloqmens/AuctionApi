using FluentValidation;

namespace Application.Commands.Bid.PlaceBid
{
    public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand>
    {
        public PlaceBidCommandValidator()
        {
            RuleFor(x => x.LotId).GreaterThan(0).WithMessage("Lot ID is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Bid amount must be greater than 0.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
        }
    }
}
