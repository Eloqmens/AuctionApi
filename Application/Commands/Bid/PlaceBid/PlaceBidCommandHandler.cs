using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Bid.PlaceBid
{
    public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Unit>
    {
        private readonly AppDbContext _context;

        public PlaceBidCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots.FindAsync(request.LotId);
            if (lot == null)
            {
                throw new KeyNotFoundException($"Lot with Id {request.LotId} not found.");
            }

            if (request.Amount <= lot.CurrentPrice)
            {
                throw new ArgumentException("Bid amount must be greater than the current price.");
            }

            lot.CurrentPrice = request.Amount;
            var bid = new Core.Entities.Bid
            {
                LotId = request.LotId,
                Amount = request.Amount,
                UserId = request.UserId
            };

            _context.Bids.Add(bid);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
