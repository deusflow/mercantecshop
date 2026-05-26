using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class ManufacturerRepository : Repository<Manufacturer>, IManufacturerRepository
{
    public ManufacturerRepository(SnipeItContext context) : base(context) { }

    public async Task<IEnumerable<Manufacturer>> GetAllActiveAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(m => m.DeletedAt == null)
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<Manufacturer?> GetActiveByIdAsync(uint id)
    {
        return await _dbSet.AsNoTracking()
            .Where(m => m.Id == id && m.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetProductsCountAsync(uint manufacturerId)
    {
        return await (
            from asset in _context.Assets
            join model in _context.Models on asset.ModelId equals (int?)model.Id
            where model.ManufacturerId == (int)manufacturerId 
                  && asset.DeletedAt == null 
                  && model.DeletedAt == null
            select asset
        ).CountAsync();
    }
}
