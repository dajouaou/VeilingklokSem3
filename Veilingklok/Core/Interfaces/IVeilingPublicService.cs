namespace Veilingklok.Core.Interfaces
{
    public interface IVeilingPublicService
    {
        Task<List<string>> GetBeschikbareVeildagenAsync();
    }
}
