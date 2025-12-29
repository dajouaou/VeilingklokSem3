namespace Veilingklok.Features.Home.Dtos;

public sealed class HomeDto
{
    public string AppName { get; set; } = string.Empty;
    public HomeHeaderDto Header { get; set; } = new();
    public HomeHeroDto Hero { get; set; } = new();
    public List<HomeRoleInfoDto> Roles { get; set; } = new();
    public HomeFooterDto Footer { get; set; } = new();
}

public sealed class HomeHeaderDto
{
    public HomeActionDto? RightLink { get; set; }
}

public sealed class HomeHeroDto
{
    public string Kicker { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public HomeActionDto PrimaryAction { get; set; } = new();
    public HomeActionDto? SecondaryAction { get; set; }
}

public sealed class HomeActionDto
{
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
}

public sealed class HomeRoleInfoDto
{
    public string RoleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class HomeFooterDto
{
    public string SecurityNote { get; set; } = string.Empty;
    public string Copyright { get; set; } = string.Empty;
}