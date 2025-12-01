namespace Veilingklok.Features.Auth.Dtos
{
    public record RegisterRequest
  (
      string Email,
      string Password,
      string Voornaam,
      string Achternaam
  );
}
