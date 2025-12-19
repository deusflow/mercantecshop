using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class StatusLabelRepository : Repository<StatusLabel>, IStatusLabelRepository
{
    public StatusLabelRepository(SnipeItContext context) : base(context) { }

    public async Task<IEnumerable<StatusLabel>> GetAllActiveAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.DeletedAt == null)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }

    public async Task<StatusLabel?> GetActiveByIdAsync(uint id)
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.Id == id && s.DeletedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetAssetsCountAsync(uint statusId)
    {
        return await _context.Assets
            .Where(a => a.StatusId == statusId && a.DeletedAt == null)
            .CountAsync();
    }

    public async Task<IEnumerable<StatusLabel>> GetDeployableStatusesAsync()
    {
        return await _dbSet.AsNoTracking()
            .Where(s => s.DeletedAt == null && s.Deployable == true)
            .OrderBy(s => s.Name)
            .ToListAsync();
    }
}
