using Infrastructure.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Lot.Delete
{
    public class DeleteLotCommandHandler
    {
        private readonly AppDbContext _context;

        public DeleteLotCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteLotCommand request, CancellationToken cancellationToken)
        {
            var lot = await _context.Lots.FindAsync(request.Id);
            if (lot == null)
            {
                throw new KeyNotFoundException($"Lot with Id {request.Id} not found.");
            }

            _context.Lots.Remove(lot);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
