using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Stripe;

namespace Application.Commands.Wallet.AddFunds
{
    public class AddFundsCommandHandler : IRequestHandler<AddFundsCommand, Unit>
    {
        private readonly UserManager<AppUser> _userManager;

        public AddFundsCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(AddFundsCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Id {request.UserId} not found.");
            }

            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = "usd",
                PaymentMethod = request.PaymentMethodId,
                ConfirmationMethod = "manual",
                Confirm = true,
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            
            user.Balance += request.Amount;
            await _userManager.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
