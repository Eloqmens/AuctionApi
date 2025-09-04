using MediatR;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Application.Exceptions;

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
            var lot = await _context.Lots.FirstOrDefaultAsync(lot => lot.Id == request.Id, cancellationToken);
            
            if (lot == null)
            {
                throw new NotFoundException(nameof(Core.Entities.Lot), request.Id);
            }

            lot.Title = request.Title;
            lot.Description = request.Description;
            lot.StartingPrice = request.StartingPrice;
            lot.EndTime = request.EndTime;
            lot.CategoryId = request.CategoryId;
            lot.UserId = request.UserId;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}