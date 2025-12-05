using Veilingklok.Core.Entities;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMAuditDto
{
    public string Action { get; set; } = string.Empty;
    public string Actor { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }

    public static VMAuditDto FromEntity(AuditEntry a)
    {
        return new VMAuditDto
        {
            Action = a.Action,
            Actor = a.ActorGebruiker?.Username ?? "",
            CreatedAtUtc = a.CreatedAtUtc
        };
    }
}