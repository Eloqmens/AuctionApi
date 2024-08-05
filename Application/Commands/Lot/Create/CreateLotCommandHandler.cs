using Infrastructure.Data;
using Core.Entities;
using MediatR;

namespace Application.Commands.Lot.Create
{
    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateLotCommandHandler(AppDbContext context)
        {
            _context = context;
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
                UserId = request.UserId
            };

            _context.Lots.Add(lot);
            await _context.SaveChangesAsync();
            return lot.Id;
        }
    }
}
