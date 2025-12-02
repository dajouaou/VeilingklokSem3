public sealed class UpdateVeilingProductValidator : AbstractValidator<UpdateVeilingProductDto>
{
    public UpdateVeilingProductValidator()
    {
        RuleFor(x => x.StartPrijs).GreaterThan(0).When(x => x.StartPrijs.HasValue);
        RuleFor(x => x.Hoeveelheid).GreaterThan(0).When(x => x.Hoeveelheid.HasValue);
        RuleFor(x => x.MinimumPrijs).GreaterThan(0).When(x => x.MinimumPrijs.HasValue);
    }
}