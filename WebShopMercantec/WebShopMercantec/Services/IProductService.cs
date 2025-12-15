using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services
{
    
    public interface IProductService
    {
        
        // IEnumerable — это просто список, по которому можно пройтись (перечисление).
        Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
        
        // Метод для получения одного товара по ID
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}