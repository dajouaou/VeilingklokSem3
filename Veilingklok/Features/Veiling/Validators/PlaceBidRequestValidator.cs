namespace Veilingklok.Features.Veiling.Validators;

using FluentValidation;
using Veilingklok.Features.Veiling.Dtos;

public sealed class PlaceBidRequestValidator : AbstractValidator<PlaceBidRequestDto>
{
    public PlaceBidRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.VeilingProductId).GreaterThan(0);
    }
}