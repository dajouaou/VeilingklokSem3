namespace Veilingklok.Features.Auth.Dtos
{

    public record LoginRequest
    (
        string Email,
        string Password
    );

}
