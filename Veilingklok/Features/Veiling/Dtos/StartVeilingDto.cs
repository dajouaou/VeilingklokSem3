namespace Veilingklok.Features.Veiling.Dtos
{
    public class StartVeilingDto
    {
        public DateTime Veildatum { get; set; }
        public DateTime LeverDatum { get; set; }
        public TimeSpan? StartTijd { get; set; }
    }
}
