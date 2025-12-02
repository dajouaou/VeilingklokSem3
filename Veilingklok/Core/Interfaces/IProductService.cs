using Veilingklok.Core.Shared;
using Veilingklok.Features.Producten.Dtos;

namespace Veilingklok.Core.Interfaces;

public interface IProductService
{
    Task<Result<ProductDto>> CreateAsync(CreateProductDto dto);
    Task<Result<ProductDto>> GetByIdAsync(int id);
    Task<Result<List<ProductDto>>> GetByAanvoerderAsync(int aanvoerderId);
    Task<Result<List<ProductDto>>> GetAllAsync();
    Task<Result<ProductDto>> UpdateAsync(int id, UpdateProductDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}