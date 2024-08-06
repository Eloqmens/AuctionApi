using Infrastructure.Data;
using MediatR;

namespace Application.Commands.Category.Create
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateCategoryCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Core.Entities.Category { Name = request.Name };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return category.Id;
        }
    }
}
