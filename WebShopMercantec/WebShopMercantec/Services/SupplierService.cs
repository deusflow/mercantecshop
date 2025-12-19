using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

public class SupplierService : ISupplierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(IUnitOfWork unitOfWork, ILogger<SupplierService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        _logger.LogInformation("Getting all suppliers");
        var suppliers = await _unitOfWork.Suppliers.GetAllActiveAsync();
        var supplierDtos = SupplierMapping.MapToDtos(suppliers).ToList();
        _logger.LogInformation("Found {Count} suppliers", supplierDtos.Count);
        return supplierDtos;
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
    {
        _logger.LogInformation("Getting supplier with ID: {SupplierId}", id);
        var supplier = await _unitOfWork.Suppliers.GetActiveByIdAsync((uint)id);
        
        if (supplier == null)
        {
            _logger.LogWarning("Supplier not found: {SupplierId}", id);
            throw new NotFoundException("Supplier", id);
        }
        
        return SupplierMapping.MapToDto(supplier);
    }
}
