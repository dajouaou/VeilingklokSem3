using FluentValidation;
using Veilingklok.Features.Auth.Dtos;

public sealed class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username of e-mail is verplicht.");

        RuleFor(x => x.Wachtwoord)
            .NotEmpty().WithMessage("Wachtwoord is verplicht.");
    }
}