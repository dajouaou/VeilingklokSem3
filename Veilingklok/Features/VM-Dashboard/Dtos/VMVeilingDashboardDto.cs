using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;
using Veilingklok.Core.Entities;


namespace Veilingklok.Features.VM.Dtos;

public sealed class VMVeilingDashboardDto
{
    public int VeilingId { get; set; }
    public string VMNaam { get; set; } = string.Empty;
    public VMCurrentProductDto? Current { get; set; }
    public List<VMVeilingProductDto> Queue { get; set; } = new();
    public List<VMAuditDto> Audit { get; set; } = new();

    public static VMVeilingDashboardDto FromEntity(Core.Entities.Veiling v)
    {
        return new VMVeilingDashboardDto
        {
            VeilingId = v.Id,
            VMNaam = v.VM?.Gebruiker?.Username ?? "",
            Current = v.CurrentVeilingProduct != null 
                ? VMCurrentProductDto.FromEntity(v.CurrentVeilingProduct)
                : null,
            Queue = v.VeilingProducten
                .Where(p => p.Status == VeilingProductStatus.Queued)
                .OrderBy(p => p.Volgorde)
                .Select(VMVeilingProductDto.FromEntity)
                .ToList(),
            Audit = v.AuditEntries
                .OrderByDescending(a => a.CreatedAtUtc)
                .Select(VMAuditDto.FromEntity)
                .ToList()
        };
    }
}