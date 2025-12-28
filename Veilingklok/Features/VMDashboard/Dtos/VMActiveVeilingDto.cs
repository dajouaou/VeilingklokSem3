using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMActiveVeilingDto
{
    public int Id { get; set; }
    public VeilingStatus Status { get; set; }
    public int? CurrentVeilingProductId { get; set; }

    public string? CurrentProductNaam { get; set; }
    public decimal? CurrentPrijs { get; set; }

    public static VMActiveVeilingDto FromEntity(Core.Entities.Veiling v)
    {
        return new VMActiveVeilingDto
        {
            Id = v.Id,
            Status = v.Status,
            CurrentVeilingProductId = v.CurrentVeilingProductId,
            CurrentProductNaam = v.CurrentVeilingProduct?.Product?.Naam,
            CurrentPrijs = v.CurrentVeilingProduct?.HuidigePrijs
        };
    }
}