using FluentValidation;
using Veilingklok.Features.Producten.Dtos;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductValidator()
    {
        RuleFor(x => x.Naam)
            .NotEmpty()
            .MaximumLength(128);

        RuleFor(x => x.Categorie)
            .MaximumLength(64);

        RuleFor(x => x.Beschrijving)
            .MaximumLength(512);

        RuleForEach(x => x.FotoUrls)
            .Must(uri => Uri.IsWellFormedUriString(uri, UriKind.Absolute))
            .WithMessage("Ongeldige foto-url.");

        // Concurrency verplicht
        RuleFor(x => x.RowVersion)
            .NotNull()
            .Must(v => v.Length > 0)
            .WithMessage("Concurrency token ontbreekt.");
    }
}