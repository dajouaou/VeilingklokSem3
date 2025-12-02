using Veilingklok.Core.Shared;
using Veilingklok.Features.Koper.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IKoperService
{
    Task<Result<KoperDto>> CreateAsync(CreateKoperDto dto);

    Task<Result<KoperDto>> GetByIdAsync(int id);

    Task<Result<KoperDto>> GetByUserIdAsync(int gebruikerId);

    Task<Result<List<KoperDto>>> GetAllAsync();

    Task<Result<KoperDto>> UpdateAsync(int id, UpdateKoperDto dto);

    Task<Result<bool>> DeleteAsync(int id);
}