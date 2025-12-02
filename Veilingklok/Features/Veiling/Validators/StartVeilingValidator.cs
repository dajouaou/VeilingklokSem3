using FluentValidation;
using Veilingklok.Features.Veiling.Dtos;

public sealed class StartVeilingValidator : AbstractValidator<StartVeilingDto>
{
    public StartVeilingValidator()
    {
        RuleFor(x => x.StartTijdUtc).NotEmpty();
        RuleFor(x => x.VeilingProductIds).NotEmpty();
    }
}