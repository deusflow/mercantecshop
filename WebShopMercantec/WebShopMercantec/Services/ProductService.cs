using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs; // Обязательно для DTO

namespace WebShopMercantec.Services
{
    public class ProductService : IProductService
    {
        private readonly SnipeItContext _context;

        public ProductService(SnipeItContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAvailableProductsAsync()
        {
            // LINQ ЗАПРОС
            // Мы используем ручной JOIN, так как в моделях нет навигационных свойств
            
            var query = from asset in _context.Assets.AsNoTracking()
                        join model in _context.Models.AsNoTracking()
                        // FIX: Приводим ID модели к int?, так как asset.ModelId это int?
                        // Это ключевой момент для стыковки типов
                        on asset.ModelId equals (int?)model.Id 
                        
                        // ФИЛЬТРЫ
                        where (asset.StatusId ?? 0) == 1       // Статус 1 (Готов)
                           && (asset.Archived ?? false) == false // Не в архиве (безопасная проверка на null)
                        
                        select new ProductDto
                        {
                            // FIX: Явное приведение uint -> int
                            // unchecked означает "если число слишком большое, просто обрежь его, не падай с ошибкой"
                            Id = unchecked((int)asset.Id), 
                            
                            Name = asset.Name ?? "Unknown Product",
                            AssetTag = asset.AssetTag ?? "No Tag",
                            
                            // Данные из таблицы Model
                            ModelName = model.Name ?? "Unknown Model", 
                            
                            // Безопасное получение данных
                            Serial = asset.Serial ?? "N/A", 
                            Image = asset.Image, // Картинка может быть null, это нормально
                            
                            StatusId = (int)(asset.StatusId ?? 0)
                        };

            return await query.ToListAsync();
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            // Здесь та же логика с типами
            
            var query = from asset in _context.Assets.AsNoTracking()
                        join model in _context.Models.AsNoTracking()
                        on asset.ModelId equals (int?)model.Id
                        // FIX: Сравниваем (int)asset.Id с нашим id
                        where (int)asset.Id == id
                        select new ProductDto
                        {
                            Id = (int)asset.Id,
                            Name = asset.Name ?? "Unknown",
                            AssetTag = asset.AssetTag ?? "No Tag",
                            ModelName = model.Name ?? "Unknown Model",
                            Serial = asset.Serial ?? "N/A",
                            Image = asset.Image,
                            StatusId = (int)(asset.StatusId ?? 0)
                        };

            return await query.FirstOrDefaultAsync();
        }
    }
}