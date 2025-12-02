using FluentValidation;
using Veilingklok.Features.Veiling.Dtos;

public sealed class BidCreateValidator : AbstractValidator<BidCreateDto>
{
    public BidCreateValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Bedrag moet groter zijn dan 0.");
    }
}