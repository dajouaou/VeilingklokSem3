using Veilingklok.Features.Home.Dtos;

namespace Veilingklok.Features.Home;

public sealed class HomeService : IHomeService
{
    public HomeDto GetHome()
    {
        return new HomeDto
        {
            AppName = "Digitale Veilingklok",
            Title = "Digitale veilingomgeving voor de sierteelt",
            Subtitle = "Realtime, betrouwbaar en professioneel",
            PrimaryAction = new HomeActionDto
            {
                Label = "Inloggen",
                Href = "/login"
            },
            SecondaryAction = new HomeActionDto
            {
                Label = "Account aanmaken",
                Href = "/register"
            },
            Roles = new List<HomeRoleInfoDto>
            {
                new()
                {
                    RoleName = "Koper",
                    Description = "Biedt in realtime op het actuele aanbod."
                },
                new()
                {
                    RoleName = "Aanvoerder",
                    Description = "Beheert het aanbod en volgt verkoopstatus."
                },
                new()
                {
                    RoleName = "Veilingmeester",
                    Description = "Start, pauzeert en bestuurt de veiling."
                }
            },
            Footer = new HomeFooterDto
            {
                SecurityNote = "Beveiligde verbinding",
                Copyright = "© Digitale Veilingklok"
            }
        };
    }
}