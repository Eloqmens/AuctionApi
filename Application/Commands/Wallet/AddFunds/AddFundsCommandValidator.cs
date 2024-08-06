using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Wallet.AddFunds
{
    public class AddFundsCommandValidator : AbstractValidator<AddFundsCommand>
    {
        public AddFundsCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
            RuleFor(x => x.PaymentMethodId).NotEmpty().WithMessage("Payment method ID is required.");
        }
    }
}
