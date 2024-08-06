using Infrastructure.Data;
using MediatR;

namespace Application.Commands.Category.Delete
{
    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Unit>
    {
        private readonly AppDbContext _context;

        public DeleteCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with Id {request.Id} not found.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
