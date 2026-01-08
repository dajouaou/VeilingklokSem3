using Veilingklok.Core.Enums;

namespace Veilingklok.Features.Veiling.Dtos
{
    public class VeilingPublicDto
    {
        public int Id { get; set; }

        public DateTime Veildatum { get; set; }     // <-- dit is belangrijk

        public TimeSpan StartTijd { get; set; }

        public DateTime? EindTijd { get; set; }

        public VeilingStatus Status { get; set; }

        public int? HuidigProductId { get; set; }
    }
}
