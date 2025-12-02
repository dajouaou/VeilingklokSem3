// Features/Veiling/Dtos/CreateVeilingDto.cs
namespace Veilingklok.Features.Veiling.Dtos;

public sealed class CreateVeilingDto
{
    public int VeilingmeesterId { get; set; }
    public string? Locatie { get; set; }
}