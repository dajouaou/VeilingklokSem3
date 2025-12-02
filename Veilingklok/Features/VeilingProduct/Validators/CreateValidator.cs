using FluentValidation;

public sealed class CreateVeilingProductValidator : AbstractValidator<CreateVeilingProductDto>
{
    public CreateVeilingProductValidator()
    {
        RuleFor(x => x.VeilingId).GreaterThan(0);
        RuleFor(x => x.ProductId).GreaterThan(0);

        RuleFor(x => x.Hoeveelheid).GreaterThan(0);
        RuleFor(x => x.StartPrijs).GreaterThan(0);
        RuleFor(x => x.MinimumPrijs).GreaterThan(0);

        RuleFor(x => x.ClockDurationMs).GreaterThan(500);
        RuleFor(x => x.ClockTickMs).GreaterThan(0);
        RuleFor(x => x.PriceDropPerTick).GreaterThan(0);
    }
}