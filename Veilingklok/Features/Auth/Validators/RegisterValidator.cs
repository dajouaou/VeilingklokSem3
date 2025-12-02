using FluentValidation;
using Veilingklok.Features.Auth.Dtos;

public sealed class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is verplicht.")
            .MinimumLength(3).MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-mail is verplicht.")
            .EmailAddress().WithMessage("E-mail is ongeldig.");

        RuleFor(x => x.Wachtwoord)
            .NotEmpty().WithMessage("Wachtwoord is verplicht.")
            .MinimumLength(6).WithMessage("Wachtwoord moet minimaal 6 karakters zijn.");
    }
}