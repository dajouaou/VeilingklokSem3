using FluentValidation;
using Veilingklok.Features.VeilingmeesterDashboard.Dtos;

namespace Veilingklok.Features.VeilingmeesterDashboard.Validators;

public sealed class CreateAuditEntryValidator : AbstractValidator<CreateAuditEntryDto>
{
    public CreateAuditEntryValidator()
    {
        RuleFor(x => x.VeilingId).GreaterThan(0);
        RuleFor(x => x.ActorGebruikerId).GreaterThan(0);
        RuleFor(x => x.Action).NotEmpty().MaximumLength(64);
        RuleFor(x => x.Details).MaximumLength(1024);
    }
}