// Veilingklok/Features/VM/Dtos/VMVeilingDashboardDto.cs
using System.Collections.Generic;
using System.Linq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMVeilingDashboardDto
{
    public int VeilingId { get; set; }
    public string VeilingNaam { get; set; } = string.Empty;
    public string Locatie { get; set; } = string.Empty;
    public VeilingStatus Status { get; set; }

    public string VMNaam { get; set; } = string.Empty;

    public VMCurrentProductDto? Current { get; set; }
    public List<VMVeilingProductDto> Queue { get; set; } = new();
    public List<VMAuditDto> Audit { get; set; } = new();

    public int TotaalProducten { get; set; }
    public int ProductenInQueue { get; set; }
    public int VerkochteProducten { get; set; }
    public int TotaalBiedingen { get; set; }

    public static VMVeilingDashboardDto FromEntity(Core.Entities.Veiling v)
    {
        var alleProducten = v.VeilingProducten ?? new List<VeilingProduct>();

        var queue = alleProducten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .Select(VMVeilingProductDto.FromEntity)
            .ToList();

        var audit = v.AuditEntries
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(VMAuditDto.FromEntity)
            .ToList();

        var totaalBiedingen = alleProducten.Sum(p => p.Bids.Count);

        return new VMVeilingDashboardDto
        {
            VeilingId = v.Id,
            VeilingNaam = v.Naam,
            Locatie = v.Locatie,
            Status = v.Status,
            VMNaam = v.VM?.Gebruiker?.Username ?? string.Empty,
            Current = v.CurrentVeilingProduct != null
                ? VMCurrentProductDto.FromEntity(v.CurrentVeilingProduct)
                : null,
            Queue = queue,
            Audit = audit,
            TotaalProducten = alleProducten.Count,
            ProductenInQueue = queue.Count,
            VerkochteProducten = alleProducten.Count(p => p.Status == VeilingProductStatus.Sold),
            TotaalBiedingen = totaalBiedingen
        };
    }
}
