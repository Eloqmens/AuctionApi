using MediatR;
using Infrastructure.Data;

namespace Application.Commands.Lot.Update
{
    public class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand>
    {
        private readonly AppDbContext _context;

        public UpdateLotCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateLotCommand request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots.FindAsync(request.Id);
            if (lot == null)
            {
                throw new KeyNotFoundException($"Lot with Id {request.Id} not found.");
            }

            lot.Title = request.Title;
            lot.Description = request.Description;
            lot.StartingPrice = request.StartingPrice;
            lot.CurrentPrice = request.CurrentPrice;
            lot.EndTime = request.EndTime;
            lot.CategoryId = request.CategoryId;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}