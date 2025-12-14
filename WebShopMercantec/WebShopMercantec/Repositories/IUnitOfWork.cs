using WebShopMercantec.Repositories.Specific;

namespace WebShopMercantec.Repositories;

/// <summary>
/// Unit of Work Pattern Interface
/// 
/// ЧТО ЭТО? 
/// Unit of Work - это паттерн, который координирует работу нескольких репозиториев
/// и гарантирует, что все изменения сохраняются в БД одной транзакцией.
/// 
/// ЗАЧЕМ?
/// 1. Централизованный доступ ко всем репозиториям
/// 2. Одна точка для сохранения всех изменений (SaveChangesAsync)
/// 3. Поддержка транзакций (все или ничего)
/// 4. Упрощает тестирование
/// 
/// КАК ИСПОЛЬЗОВАТЬ?
/// Inject IUnitOfWork в сервис:
/// 
/// public class OrderService
/// {
///     private readonly IUnitOfWork _unitOfWork;
///     
///     public async Task CreateOrder(int userId, int productId)
///     {
///         // Начинаем транзакцию
///         await _unitOfWork.BeginTransactionAsync();
///         
///         try 
///         {
///             // Работаем с несколькими репозиториями
///             var user = await _unitOfWork.Users.GetByIdAsync(userId);
///             var product = await _unitOfWork.Products.GetByIdAsync(productId);
///             
///             // Создаем заказ
///             var order = new Order { ... };
///             await _unitOfWork.Orders.AddAsync(order);
///             
///             // ВАЖНО: Сохраняем ВСЕ изменения одной транзакцией
///             await _unitOfWork.SaveChangesAsync();
///             
///             // Коммитим транзакцию
///             await _unitOfWork.CommitTransactionAsync();
///         }
///         catch
///         {
///             // Если ошибка - откатываем все изменения
///             await _unitOfWork.RollbackTransactionAsync();
///             throw;
///         }
///     }
/// }
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // === РЕПОЗИТОРИИ ===
    // Каждый репозиторий - это специализированный "работник" для своей таблицы
    
    /// <summary>
    /// Репозиторий для работы с пользователями (таблица Users)
    /// </summary>
    IUserRepository Users { get; }
    
    /// <summary>
    /// Репозиторий для работы с продуктами (таблица Assets)
    /// </summary>
    IProductRepository Products { get; }
    
    /// <summary>
    /// Репозиторий для работы с заказами (таблица CheckoutRequests)
    /// </summary>
    IOrderRepository Orders { get; }
    
    /// <summary>
    /// Репозиторий для работы с аксессуарами (таблица Accessories)
    /// </summary>
    IAccessoryRepository Accessories { get; }
    
    // Можно добавить другие репозитории по мере необходимости:
    // IRepository<Category> Categories { get; }
    // IRepository<Manufacturer> Manufacturers { get; }
    // и т.д.
    
    
    // === МЕТОДЫ УПРАВЛЕНИЯ ===
    
    /// <summary>
    /// Сохранить все изменения в базе данных
    /// Вызывает DbContext.SaveChangesAsync()
    /// </summary>
    /// <returns>Количество измененных записей</returns>
    Task<int> SaveChangesAsync();
    
    /// <summary>
    /// Начать транзакцию БД
    /// Используйте, когда нужно выполнить несколько операций атомарно
    /// (все успешно или все откатывается)
    /// </summary>
    Task BeginTransactionAsync();
    
    /// <summary>
    /// Зафиксировать (commit) транзакцию
    /// Применяет все изменения в БД
    /// </summary>
    Task CommitTransactionAsync();
    
    /// <summary>
    /// Откатить (rollback) транзакцию
    /// Отменяет все изменения, сделанные в рамках транзакции
    /// </summary>
    Task RollbackTransactionAsync();
}