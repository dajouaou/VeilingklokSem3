using System;
using System.Collections.Generic;
using Veilingklok.Core.Enums;

namespace Veilingklok.Core.Entities;

public class Veiling
{
    public int Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public KlokLocatie Locatie { get; set; }

    public DateTime Datum { get; set; }
    public TimeSpan StartTijd { get; set; }

    public VeilingStatus Status { get; set; } = VeilingStatus.Scheduled;

    public DateTime? StartTijdUtc { get; set; }
    public DateTime? EindTijdUtc { get; set; }

    public int? CurrentVeilingProductId { get; set; }
    public VeilingProduct? CurrentVeilingProduct { get; set; }

    public int? VMId { get; set; }
    public VM? VM { get; set; }

    public List<VeilingProduct> VeilingProducten { get; set; } = new();
    public List<AuditEntry> AuditEntries { get; set; } = new();
}