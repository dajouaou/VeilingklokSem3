using System;

namespace Veilingklok.Features.Veiling.Dtos;

public sealed class CreateVeilingFromLeverdatumDto
{
    public DateTime Veildatum { get; set; }
    public string Leverdatum { get; set; } = string.Empty;
    public TimeSpan? StartTijd { get; set; }
}