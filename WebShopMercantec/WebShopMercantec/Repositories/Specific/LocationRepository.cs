using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
namespace WebShopMercantec.Repositories.Specific;
public class LocationRepository : Repository<Location>, ILocationRepository
{
    public LocationRepository(SnipeItContext context) : base(context) { }
    public async Task<IEnumerable<Location>> GetAllActiveAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(l => l.DeletedAt == null)
            .OrderBy(l => l.Name)
            .ToListAsync();
    }
    public async Task<Location?> GetActiveByIdAsync(uint id)
    {
        return await _dbSet.AsNoTracking()
            .Where(l => l.Id == id && l.DeletedAt == null)
            .FirstOrDefaultAsync();
    }
}
