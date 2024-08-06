using MediatR;

namespace Application.Commands.Category.Delete
{
    public class DeleteCategoryCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }
}
