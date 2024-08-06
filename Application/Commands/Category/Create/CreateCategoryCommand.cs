using MediatR;

namespace Application.Commands.Category.Create
{
    public class CreateCategoryCommand : IRequest<int>
    {
        public string Name { get; set; }
    }
}
