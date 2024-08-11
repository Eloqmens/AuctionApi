using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Wallet.AddWallet
{
    public class AddCardCommandHandler : IRequestHandler<AddCardCommand, Unit>
    {
        private readonly UserManager<AppUser> _userManager;

        public AddCardCommandHandler(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Unit> Handle(AddCardCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with Id {request.UserId} not found.");
            }

            var wallet = await _userManager.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => u.Wallet)
                .FirstOrDefaultAsync(cancellationToken);

            if (wallet == null)
            {
                throw new KeyNotFoundException($"Wallet for user with Id {request.UserId} not found.");
            }

            wallet.CardNumber = request.CardNumber;
            await _userManager.UpdateAsync(user);

            return Unit.Value;
        }
    }
}
