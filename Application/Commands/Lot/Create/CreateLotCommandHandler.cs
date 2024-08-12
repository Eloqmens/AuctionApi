using Infrastructure.Data;
using Core.Entities;
using MediatR;
using Application.Interfaces;

namespace Application.Commands.Lot.Create
{
    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, int>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CreateLotCommandHandler(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<int> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            var lot = new Core.Entities.Lot
            {
                Title = request.Title,
                Description = request.Description,
                StartingPrice = request.StartingPrice,
                EndTime = request.EndTime,
                CategoryId = request.CategoryId,
                UserId = _currentUserService.UserId
            };

            _context.Lots.Add(lot);
            await _context.SaveChangesAsync(cancellationToken);
            return lot.Id;
        }
    }

}
