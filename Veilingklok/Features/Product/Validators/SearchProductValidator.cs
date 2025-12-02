using FluentValidation;
using Veilingklok.Features.Producten.Dtos;

public sealed class SearchProductValidator : AbstractValidator<SearchProductDto>
{
    public SearchProductValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(5, 100);

        RuleFor(x => x.Naam)
            .MaximumLength(128);

        RuleFor(x => x.Categorie)
            .MaximumLength(64);

        RuleFor(x => x.Kleur)
            .MaximumLength(64);
    }
}