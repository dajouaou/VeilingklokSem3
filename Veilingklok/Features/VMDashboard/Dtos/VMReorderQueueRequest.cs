using System;

namespace Veilingklok.Features.VM.Dtos;

public sealed class VMReorderQueueRequest
{
    public int[] OrderedVeilingProductIds { get; set; } = Array.Empty<int>();
}
