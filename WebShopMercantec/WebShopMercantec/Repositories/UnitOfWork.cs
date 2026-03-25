using Microsoft.EntityFrameworkCore.Storage;
using WebShopMercantec.Models;
using WebShopMercantec.Repositories.Specific;

namespace WebShopMercantec.Repositories;

/// <summary>
/// Unit of Work Implementation
/// Реализация паттерна Unit of Work для координации работы репозиториев.
/// Все репозитории инжектируются через DI (не создаются вручную через new).
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly SnipeItContext _context;
    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Конструктор — все зависимости через DI
    /// </summary>
    public UnitOfWork(
        SnipeItContext context,
        IUserRepository users,
        IProductRepository products,
        IOrderRepository orders,
        IAccessoryRepository accessories,
        ICategoryRepository categories,
        IManufacturerRepository manufacturers,
        ISupplierRepository suppliers,
        ILocationRepository locations,
        IStatusLabelRepository statusLabels)
    {
        _context = context;
        Users = users;
        Products = products;
        Orders = orders;
        Accessories = accessories;
        Categories = categories;
        Manufacturers = manufacturers;
        Suppliers = suppliers;
        Locations = locations;
        StatusLabels = statusLabels;
    }

    // === РЕПОЗИТОРИИ (через DI) ===
    
    public IUserRepository Users { get; }
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public IAccessoryRepository Accessories { get; }
    public ICategoryRepository Categories { get; }
    public IManufacturerRepository Manufacturers { get; }
    public ISupplierRepository Suppliers { get; }
    public ILocationRepository Locations { get; }
    public IStatusLabelRepository StatusLabels { get; }

    /// <summary>
    /// Direct context access for WebShop-specific DbSets
    /// </summary>
    public SnipeItContext Context => _context;

    // === МЕТОДЫ УПРАВЛЕНИЯ ===

    /// <summary>
    /// Сохранить все изменения в БД
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Начать транзакцию БД
    /// </summary>
    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
            return;

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    /// <summary>
    /// Зафиксировать транзакцию
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
    /// </summary>
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}