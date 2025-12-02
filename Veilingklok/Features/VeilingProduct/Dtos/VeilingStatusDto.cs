public sealed class VeilingStatusDto
{
    public int VeilingId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? CurrentVeilingProductId { get; set; }
}