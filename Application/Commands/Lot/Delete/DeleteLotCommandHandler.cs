using Application.Exceptions;
using Core.Entities;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Lot.Delete
{
    public class DeleteLotCommandHandler : IRequestHandler<DeleteLotCommand>
    {
        private readonly AppDbContext _context;

        public DeleteLotCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteLotCommand request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots
                .Include(l => l.Bids)
                .FirstOrDefaultAsync(lot => lot.Id == request.Id, cancellationToken);

            if (lot == null || lot.UserId != request.UserId)
            {
                throw new NotFoundException(nameof(Core.Entities.Lot), request.Id);
            }

            _context.Lots.Remove(lot);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
