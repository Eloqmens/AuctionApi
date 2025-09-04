using FluentValidation;

namespace Application.Queries.Lot.GetAll
{
    public class GetLotsQueryValidator : AbstractValidator<GetLotsQuery>
    {
        public GetLotsQueryValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("PageNumber должен быть больше 0");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("PageSize должен быть от 1 до 100");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0)
                .When(x => x.CategoryId.HasValue)
                .WithMessage("CategoryId должен быть больше 0");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Поисковый запрос не должен превышать 100 символов");
        }
    }
}
