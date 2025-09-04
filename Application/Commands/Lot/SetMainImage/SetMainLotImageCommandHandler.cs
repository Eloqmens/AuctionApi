using Application.Exceptions;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.Lot.SetMainImage
{
    public class SetMainLotImageCommandHandler : IRequestHandler<SetMainLotImageCommand, Unit>
    {
        private readonly AppDbContext _context;

        public SetMainLotImageCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(SetMainLotImageCommand request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots.Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.Id == request.LotId, cancellationToken);
            if (lot == null)
            {
                throw new NotFoundException("Lot", request.LotId);
            }

            var image = lot.Images.FirstOrDefault(i => i.Id == request.ImageId);
            if (image == null)
            {
                throw new NotFoundException("LotImage", request.ImageId);
            }

            foreach (var img in lot.Images)
            {
                img.IsMain = img.Id == request.ImageId;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}


