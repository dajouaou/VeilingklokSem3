namespace Veilingklok.Core.Interfaces
{
    public interface IBlobImageService
    {
        Task<string> UploadAsync(IFormFile file, CancellationToken ct = default);
    }

}
