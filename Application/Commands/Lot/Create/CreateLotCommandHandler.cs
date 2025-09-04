using Application.Exceptions;
using Application.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Lot.Create
{
    public class CreateLotCommandHandler : IRequestHandler<CreateLotCommand, int>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreateLotCommandHandler> _logger;

        public CreateLotCommandHandler(AppDbContext context, ICurrentUserService currentUserService, ILogger<CreateLotCommandHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<int> Handle(CreateLotCommand request, CancellationToken cancellationToken)
        {
            
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId, cancellationToken);

            if (!categoryExists)
            {
                throw new NotFoundException("Category", request.CategoryId);
            }

            var lot = new Core.Entities.Lot
            {
                Title = request.Title,
                Description = request.Description,
                StartingPrice = request.StartingPrice,
                CurrentPrice = request.StartingPrice,
                EndTime = request.EndTime,
                CategoryId = request.CategoryId,
                UserId = request.UserId ?? _currentUserService.UserId!,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 } // Для новых записей
            };

            _context.Lots.Add(lot);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Создан новый лот {LotId} пользователем {UserId}", lot.Id, lot.UserId);

            return lot.Id;
        }
    }
}
