using MediatR;
using Infrastructure.Data;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Application.Exceptions;

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
            var lot = await _context.Lots.FirstOrDefaultAsync(lot => lot.Id == request.Id, cancellationToken);
            if (lot == null || lot.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Core.Entities.Lot), request.Id);
            }

            lot.Title = request.Title;
            lot.Description = request.Description;
            lot.StartingPrice = request.StartingPrice;
            lot.EndTime = request.EndTime;
            lot.CategoryId = request.CategoryId;
            lot.UserId = _currentUserService.UserId;

            _context.Lots.Update(lot);
            await _context.SaveChangesAsync(cancellationToken);
        }


    }
}