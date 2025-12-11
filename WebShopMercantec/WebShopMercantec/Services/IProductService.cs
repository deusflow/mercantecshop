using WebShopMercantec.Shared.DTOs;

namespace WebShopMercantec.Services
{
    // Интерфейс всегда начинается с буквы "I" (Convention)
    public interface IProductService
    {
        // Task<...> — это обещание (Promise). Означает "Я верну результат, но чуть позже, асинхронно".
        // IEnumerable — это просто список, по которому можно пройтись (перечисление).
        Task<IEnumerable<ProductDto>> GetAvailableProductsAsync();
        
        // Метод для получения одного товара по ID
        Task<ProductDto?> GetProductByIdAsync(int id);
    }
}