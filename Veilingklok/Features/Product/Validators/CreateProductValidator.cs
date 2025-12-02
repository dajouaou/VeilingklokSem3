using FluentValidation;
using Veilingklok.Features.Producten.Dtos;

public sealed class CreateProductValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductValidator()
    {
        RuleFor(x => x.AanvoerderId)
            .GreaterThan(0);

        RuleFor(x => x.Naam)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Categorie)
            .MaximumLength(64);

        RuleFor(x => x.Beschrijving)
            .MaximumLength(512);

        // Minimaal 1 foto verplicht (WCAG requirement)
        RuleFor(x => x.FotoUrls)
            .NotEmpty()
            .WithMessage("Minimaal één productfoto is verplicht.");

        RuleForEach(x => x.FotoUrls)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Ongeldige foto-url.");

        RuleFor(x => x.Kleur)
            .MaximumLength(64);

        RuleFor(x => x.Hoogte)
            .MaximumLength(32);
    }
}