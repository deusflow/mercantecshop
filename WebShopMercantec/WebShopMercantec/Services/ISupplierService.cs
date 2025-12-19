using WebShopMercantec.Shared.DTOs;
namespace WebShopMercantec.Services;
public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
    Task<SupplierDto?> GetSupplierByIdAsync(int id);
}
