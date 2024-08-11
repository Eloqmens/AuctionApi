using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Wallet.AddWallet
{
    public class AddCardCommandValidator : AbstractValidator<AddCardCommand>
    {
        public AddCardCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.CardNumber).NotEmpty().WithMessage("Card number is required.");
        }
    }
}
