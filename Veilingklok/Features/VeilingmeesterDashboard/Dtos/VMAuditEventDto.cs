using System;
using Veilingklok.Core.Entities;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMAuditEventDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public string ActorNaam { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public static VMAuditEventDto FromEntity(AuditEntry a)
    {
        var fullName = a.ActorGebruiker?.FullName;
        var username = a.ActorGebruiker?.Username;

        var actorTech = !string.IsNullOrWhiteSpace(username) ? username : fullName;
        var actorNaam = !string.IsNullOrWhiteSpace(fullName) ? fullName : username;

        return new VMAuditEventDto
        {
            Id = a.Id,
            Action = a.Action,
            Actor = actorTech ?? string.Empty,
            ActorNaam = actorNaam ?? string.Empty,
            CreatedAtUtc = a.CreatedAtUtc
        };
    }
}