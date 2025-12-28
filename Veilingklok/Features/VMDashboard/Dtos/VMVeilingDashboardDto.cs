using System;
using System.Collections.Generic;
using System.Linq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMVeilingDashboardDto
{
    public int VeilingId { get; set; }
    public string VeilingNaam { get; set; } = string.Empty;
    public KlokLocatie Locatie { get; set; }
    public VeilingStatus Status { get; set; }
    public string VMNaam { get; set; } = string.Empty;

    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }

    public VMCurrentProductDto? Current { get; set; }
    public List<VMQueueItemDto> Queue { get; set; } = new();
    public List<VMAuditEventDto> Audit { get; set; } = new();

    public VMStatsDto Stats { get; set; } = new();

    public static VMVeilingDashboardDto FromEntity(Core.Entities.Veiling v)
    {
        var producten = v.VeilingProducten ?? new List<VeilingProduct>();

        var queue = producten
            .Where(p => p.Status == VeilingProductStatus.Queued)
            .OrderBy(p => p.Volgorde)
            .ThenBy(p => p.Id)
            .Select(VMQueueItemDto.FromEntity)
            .ToList();

        var audit = (v.AuditEntries ?? new List<AuditEntry>())
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(VMAuditEventDto.FromEntity)
            .ToList();

        var totalBids = producten.Sum(p => (p.Bids ?? new List<Bid>()).Count);

        var stats = new VMStatsDto
        {
            TotaalProducten = producten.Count,
            ProductenInQueue = producten.Count(p => p.Status == VeilingProductStatus.Queued),
            VerkochteProducten = producten.Count(p => p.Status == VeilingProductStatus.Sold),
            OvergeslagenProducten = producten.Count(p => p.Status == VeilingProductStatus.Skipped),
            TotaalBiedingen = totalBids
        };

        return new VMVeilingDashboardDto
        {
            VeilingId = v.Id,
            VeilingNaam = v.Naam ?? string.Empty,
            Locatie = v.Locatie,
            Status = v.Status,
            VMNaam =
                v.VM?.Naam
                ?? v.VM?.Gebruiker?.FullName
                ?? v.VM?.Gebruiker?.Username
                ?? string.Empty,
            Datum = v.Datum,
            StartTijd = v.StartTijd,
            StartTijdUtc = v.StartTijdUtc,
            EindTijdUtc = v.EindTijdUtc,
            Current = v.CurrentVeilingProduct != null ? VMCurrentProductDto.FromEntity(v.CurrentVeilingProduct) : null,
            Queue = queue,
            Audit = audit,
            Stats = stats
        };
    }
}
