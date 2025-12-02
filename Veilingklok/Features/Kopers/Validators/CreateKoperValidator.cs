using FluentValidation;
using Veilingklok.Features.Koper.Dtos;

public sealed class CreateKoperValidator : AbstractValidator<CreateKoperDto>
{
    public CreateKoperValidator()
    {
        RuleFor(x => x.Naam).NotEmpty().MaximumLength(128);
    }
}