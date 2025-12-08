// Veilingklok/Features/VM/Dtos/VMVeilingDashboardDto.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Veilingklok.Core.Entities;
using Veilingklok.Core.Enums;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMVeilingDashboardDto
{
    public int VeilingId { get; set; }
    public string? VeilingNaam { get; set; }
    public string? Locatie { get; set; }
    public VeilingStatus Status { get; set; }
    public string? VMNaam { get; set; }

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }

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
            .ThenBy(p => p.Id)
            .Select(VMVeilingProductDto.FromEntity)
            .ToList();

        var audit = (v.AuditEntries ?? new List<AuditEntry>())
            .OrderByDescending(a => a.CreatedAtUtc)
            .Select(VMAuditDto.FromEntity)
            .ToList();

        var totaalBiedingen = alleProducten
            .Sum(p => (p.Bids ?? new List<Bid>()).Count);

        var productenInQueueCount = alleProducten.Count(p =>
            p.Status == VeilingProductStatus.Queued);

        var verkochteProductenCount = alleProducten.Count(p =>
            p.Status == VeilingProductStatus.Sold);

        return new VMVeilingDashboardDto
        {
            VeilingId = v.Id,
            VeilingNaam = v.Naam,
            Locatie = v.Locatie,
            Status = v.Status,
            VMNaam =
                v.VM?.Naam
                ?? v.VM?.Gebruiker?.FullName
                ?? v.VM?.Gebruiker?.Username
                ?? string.Empty,
            StartTijdUtc = v.StartTijdUtc,
            EindTijdUtc = v.EindTijdUtc,
            Current = v.CurrentVeilingProduct != null
                ? VMCurrentProductDto.FromEntity(v.CurrentVeilingProduct)
                : null,
            Queue = queue,
            Audit = audit,
            TotaalProducten = alleProducten.Count,
            ProductenInQueue = productenInQueueCount,
            VerkochteProducten = verkochteProductenCount,
            TotaalBiedingen = totaalBiedingen
        };
    }
}
