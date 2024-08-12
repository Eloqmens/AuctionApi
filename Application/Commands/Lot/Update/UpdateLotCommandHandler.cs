using MediatR;
using Infrastructure.Data;
using Application.Interfaces;

namespace Application.Commands.Lot.Update
{
    public class UpdateLotCommandHandler : IRequestHandler<UpdateLotCommand>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        public UpdateLotCommandHandler(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
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
            lot.UserId = _currentUserService.UserId;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}