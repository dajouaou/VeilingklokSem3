using System.ComponentModel.DataAnnotations;

namespace Veilingklok.Features.AanvoerderDashboard.Dtos
{
    public class DashboardDTO
    {
        public int Id { get; set; }

    [Required, StringLength(250)]
    public string Info { get; set; } = string.Empty;
}
}





