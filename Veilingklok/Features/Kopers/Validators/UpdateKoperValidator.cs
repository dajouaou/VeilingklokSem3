using FluentValidation;
using Veilingklok.Features.Koper.Dtos;

public sealed class UpdateKoperValidator : AbstractValidator<UpdateKoperDto>
{
    public UpdateKoperValidator()
    {
        RuleFor(x => x.Naam).NotEmpty().MaximumLength(128);
    }
}