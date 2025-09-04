using Application.Exceptions;
using Application.Interfaces;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Bid.PlaceBid
{
    public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Unit>
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<PlaceBidCommandHandler> _logger;

        public PlaceBidCommandHandler(AppDbContext context, ICurrentUserService currentUserService, ILogger<PlaceBidCommandHandler> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Unit> Handle(PlaceBidCommand request, CancellationToken cancellationToken)
        {
            // Проверяем, что пользователь авторизован
            if (string.IsNullOrEmpty(_currentUserService.UserId))
            {
                throw new BusinessException("Пользователь не авторизован");
            }

            const int maxRetries = 3;
            var attempt = 0;

            while (attempt < maxRetries)
            {
                try
                {
                    var lot = await _context.Lots.FindAsync(new object[] { request.LotId }, cancellationToken);
                    if (lot == null)
                    {
                        throw new NotFoundException("Lot", request.LotId);
                    }

                   
                    if (lot.EndTime <= DateTime.UtcNow)
                    {
                        throw new BusinessException("Аукцион по данному лоту уже завершен");
                    }

                       
                    if (lot.UserId == _currentUserService.UserId)
                    {
                        throw new BusinessException("Нельзя делать ставки на собственные лоты");
                    }

                   
                    var minIncrement = Math.Max(1000m, lot.CurrentPrice * 0.05m);
                    var minAllowed = lot.CurrentPrice + minIncrement;
                    if (request.Amount < minAllowed)
                    {
                        throw new BusinessException($"Ставка должна быть не менее {minAllowed:0} ₽");
                    }

                   
                    lot.CurrentPrice = request.Amount;
                    lot.UpdatedAt = DateTime.UtcNow;

                   
                    var bid = new Core.Entities.Bid
                    {
                        LotId = request.LotId,
                        Amount = request.Amount,
                        UserId = _currentUserService.UserId!,
                        Timestamp = DateTime.UtcNow
                    };

                    _context.Bids.Add(bid);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Пользователь {UserId} успешно сделал ставку {Amount} на лот {LotId}",
                        _currentUserService.UserId, request.Amount, request.LotId);

                    return Unit.Value;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    attempt++;
                    
                    _logger.LogWarning("Конфликт concurrency при размещении ставки (попытка {Attempt}/{MaxRetries}): {Message}",
                        attempt, maxRetries, ex.Message);

                    if (attempt >= maxRetries)
                    {
                        throw new BusinessException("Не удалось разместить ставку из-за высокой конкуренции. Попробуйте еще раз");
                    }

                   
                    foreach (var entry in _context.ChangeTracker.Entries())
                    {
                        entry.Reload();
                    }

                   
                    await Task.Delay(100 * attempt, cancellationToken);
                }
            }

            return Unit.Value;
        }
    }
}
