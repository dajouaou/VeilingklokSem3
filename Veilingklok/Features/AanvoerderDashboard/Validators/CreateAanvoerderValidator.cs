using FluentValidation;
using Veilingklok.Features.AanvoerderDashboard.Dtos;

public sealed class CreateAanvoerderValidator : AbstractValidator<CreateAanvoerderDto>
{
    public CreateAanvoerderValidator()
    {
        RuleFor(x => x.Naam)
            .NotEmpty().WithMessage("Naam is verplicht.")
            .MaximumLength(128).WithMessage("Naam mag maximaal 128 tekens hebben.");

        RuleFor(x => x.ContactInfo)
            .MaximumLength(256).WithMessage("Contactinfo mag maximaal 256 tekens hebben.");
    }
}