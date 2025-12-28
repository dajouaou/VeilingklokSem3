using System;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMAuditDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public int ActorGebruikerId { get; set; }
    public string? ActorNaam { get; set; }

    public static VMAuditDto FromEntity(Core.Entities.AuditEntry entry)
    {
        return new VMAuditDto
        {
            Id = entry.Id,
            Action = entry.Action,
            CreatedAtUtc = entry.CreatedAtUtc,
            ActorGebruikerId = entry.ActorGebruikerId,
            ActorNaam = entry.ActorGebruiker == null
                ? null
                : $"{entry.ActorGebruiker.Voornaam} {entry.ActorGebruiker.Achternaam}".Trim()
        };
    }
}