using Microsoft.EntityFrameworkCore.Storage;
using WebShopMercantec.Models;
using WebShopMercantec.Repositories.Specific;

namespace WebShopMercantec.Repositories;

/// <summary>
/// Unit of Work Implementation
/// Реализация паттерна Unit of Work для координации работы репозиториев
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SnipeItContext _context;
    private IDbContextTransaction? _transaction;
    
    // Приватные поля для lazy initialization репозиториев
    private IUserRepository? _users;
    private IProductRepository? _products;
    private IOrderRepository? _orders;
    private IAccessoryRepository? _accessories;

    /// <summary>
    /// Конструктор - принимает DbContext через Dependency Injection
    /// </summary>
    public UnitOfWork(SnipeItContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Репозиторий пользователей
    /// Lazy Loading: создается только когда первый раз используется
    /// </summary>
    public IUserRepository Users
    {
        get
        {
            // Если еще не создан - создаем
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    /// <summary>
    /// Репозиторий продуктов (Assets)
    /// </summary>
    public IProductRepository Products
    {
        get
        {
            _products ??= new ProductRepository(_context);
            return _products;
        }
    }

    /// <summary>
    /// Репозиторий заказов (CheckoutRequests)
    /// </summary>
    public IOrderRepository Orders
    {
        get
        {
            _orders ??= new OrderRepository(_context);
            return _orders;
        }
    }

    /// <summary>
    /// Репозиторий аксессуаров
    /// </summary>
    public IAccessoryRepository Accessories
    {
        get
        {
            _accessories ??= new AccessoryRepository(_context);
            return _accessories;
        }
    }

    /// <summary>
    /// Сохранить все изменения в БД
    /// Это ЕДИНСТВЕННАЯ точка, где данные реально записываются в БД!
    /// </summary>
    /// <returns>Количество измененных записей</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Начать транзакцию БД
    /// 
    /// КОГДА ИСПОЛЬЗОВАТЬ?
    /// Когда нужно выполнить несколько операций, и либо все должны пройти успешно,
    /// либо все должны откатиться.
    /// 
    /// ПРИМЕР:
    /// - Создание заказа + списание кредитов + обновление статуса товара
    /// Если что-то пойдет не так - все откатывается!
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Зафиксировать транзакцию
    /// Применяет все изменения, сделанные в рамках транзакции
    /// </summary>
    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Откатить транзакцию
    /// Отменяет ВСЕ изменения, сделанные после BeginTransactionAsync()
    /// </summary>
    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Освободить ресурсы
    /// Вызывается автоматически DI контейнером
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}