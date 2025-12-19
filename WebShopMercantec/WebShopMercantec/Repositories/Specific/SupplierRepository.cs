using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
namespace WebShopMercantec.Repositories.Specific;
public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(SnipeItContext context) : base(context) { }
    public async Task<IEnumerable<Supplier>> GetAllActiveAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.DeletedAt == null)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
    public async Task<Supplier?> GetActiveByIdAsync(uint id)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.Id == id && s.DeletedAt == null)
            .FirstOrDefaultAsync();
    }
}
