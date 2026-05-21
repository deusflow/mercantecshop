using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;

namespace WebShopMercantec.Repositories.Specific;

public class ProductRepository : Repository<Asset>, IProductRepository
{
    public ProductRepository(SnipeItContext context) : base(context)
    {
    }

    
    public async Task<IEnumerable<Asset>> GetAvailableProductsAsync()
    {
        var availabilityRules = await GetAvailabilityRulesAsync();

        return await ApplyAvailableAssetFilter(_dbSet.AsNoTracking(), availabilityRules)
            .OrderBy(a => a.Name)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId)
    {
        var modelIds = await GetModelIdsByCategoryAsync(categoryId);
        if (modelIds.Count == 0)
            return [];

        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId) &&
                a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetByModelIdAsync(int modelId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.ModelId == modelId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetByManufacturerAsync(int manufacturerId)
    {
        var modelIds = await GetModelIdsByManufacturerAsync(manufacturerId);
        if (modelIds.Count == 0)
            return [];

        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId) &&
                a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetByStatusIdAsync(int statusId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.StatusId == statusId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetByLocationIdAsync(int locationId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.LocationId == locationId && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetAssignedToUserAsync(int userId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.AssignedTo == userId &&
                a.AssignedType == "App\\Models\\User" && // Ensure assignment type
                a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> SearchProductsAsync(string searchTerm)
    {
        var term = $"%{searchTerm.Trim()}%";

        return await _dbSet
            .AsNoTracking()
            .Where(a => a.DeletedAt == null && (
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.AssetTag != null && EF.Functions.Like(a.AssetTag, term)) ||
                (a.Serial != null && EF.Functions.Like(a.Serial, term))
            ))
            .ToListAsync();
    }

    
    public async Task<(IEnumerable<Asset> Products, int TotalCount)> GetProductsPagedAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        int? statusId = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? availableOnly = true)
    {
        // Base query
        var query = _dbSet.AsNoTracking().Where(a => a.DeletedAt == null);

        // Filter: available for checkout only
        if (availableOnly == true)
        {
            var availabilityRules = await GetAvailabilityRulesAsync();
            query = ApplyAvailableAssetFilter(query, availabilityRules);
        }

        // Filter by category (via model)
        if (categoryId.HasValue)
        {
            var modelIds = await GetModelIdsByCategoryAsync(categoryId.Value);
            if (modelIds.Count == 0)
                return ([], 0);

            query = query.Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId));
        }

        // Filter by manufacturer (via model)
        if (manufacturerId.HasValue)
        {
            var modelIds = await GetModelIdsByManufacturerAsync(manufacturerId.Value);
            if (modelIds.Count == 0)
                return ([], 0);

            query = query.Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId));
        }

        // Filter by status
        if (statusId.HasValue)
        {
            query = query.Where(a => a.StatusId == statusId);
        }

        // Filter by search term
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(a =>
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.AssetTag != null && EF.Functions.Like(a.AssetTag, term)) ||
                (a.Serial != null && EF.Functions.Like(a.Serial, term))
            );
        }

        // Filter by price
        if (minPrice.HasValue)
        {
            query = query.Where(a => a.PurchaseCost >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(a => a.PurchaseCost <= maxPrice.Value);
        }

        // Count total items
        var totalCount = await query.CountAsync();

        // Fetch the page with sorting
        var products = await query
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (products, totalCount);
    }

    
    public async Task<bool> IsAvailableForCheckoutAsync(uint assetId)
    {
        var availabilityRules = await GetAvailabilityRulesAsync();

        return await _dbSet
            .AnyAsync(a =>
                a.Id == assetId &&
                a.StatusId.HasValue &&
                availabilityRules.DeployableStatusIds.Contains(a.StatusId.Value) &&
                a.ModelId.HasValue &&
                availabilityRules.RequestableModelIds.Contains(a.ModelId.Value) &&
                a.DeletedAt == null &&
                a.Requestable == 1 &&
                a.AssignedTo == null);
    }

    
    public async Task<Asset?> GetByAssetTagAsync(string assetTag)
    {
        var normalizedTag = assetTag.Trim();

        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.AssetTag != null &&
                a.AssetTag == normalizedTag &&
                a.DeletedAt == null);
    }

    
    public async Task<IEnumerable<Asset>> GetArchivedProductsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.Archived == true && a.DeletedAt == null)
            .ToListAsync();
    }

    
    public async Task<IEnumerable<Asset>> GetProductsRequiringMaintenanceAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return await _dbSet
            .AsNoTracking()
            .Where(a =>
                a.DeletedAt == null &&
                a.NextAuditDate.HasValue &&
                a.NextAuditDate <= today)
            .ToListAsync();
    }

    // === METHODS WITH RELATED DATA ===

    
    private async Task<List<AssetWithDetails>> ComposeAssetsWithDetailsAsync(List<Asset> assets)
    {
        if (assets.Count == 0)
            return [];

        var modelIds = assets
            .Where(a => a.ModelId.HasValue)
            .Select(a => (uint)a.ModelId!.Value)
            .Distinct()
            .ToList();

        var statusIds = assets
            .Where(a => a.StatusId.HasValue)
            .Select(a => (uint)a.StatusId!.Value)
            .Distinct()
            .ToList();

        var locationIds = assets
            .Where(a => a.LocationId.HasValue)
            .Select(a => (uint)a.LocationId!.Value)
            .Distinct()
            .ToList();

        var supplierIds = assets
            .Where(a => a.SupplierId.HasValue)
            .Select(a => (uint)a.SupplierId!.Value)
            .Distinct()
            .ToList();

        var models = modelIds.Count == 0
            ? []
            : await _context.Models.AsNoTracking().Where(m => modelIds.Contains(m.Id)).ToListAsync();

        var categoryIds = models
            .Where(m => m.CategoryId.HasValue)
            .Select(m => (uint)m.CategoryId!.Value)
            .Distinct()
            .ToList();

        var manufacturerIds = models
            .Where(m => m.ManufacturerId.HasValue)
            .Select(m => (uint)m.ManufacturerId!.Value)
            .Distinct()
            .ToList();

        var categories = categoryIds.Count == 0
            ? []
            : await _context.Categories.AsNoTracking().Where(c => categoryIds.Contains(c.Id)).ToListAsync();

        var manufacturers = manufacturerIds.Count == 0
            ? []
            : await _context.Manufacturers.AsNoTracking().Where(m => manufacturerIds.Contains(m.Id)).ToListAsync();

        var statusLabels = statusIds.Count == 0
            ? []
            : await _context.StatusLabels.AsNoTracking().Where(s => statusIds.Contains(s.Id)).ToListAsync();

        var locations = locationIds.Count == 0
            ? []
            : await _context.Locations.AsNoTracking().Where(l => locationIds.Contains(l.Id)).ToListAsync();

        var suppliers = supplierIds.Count == 0
            ? []
            : await _context.Suppliers.AsNoTracking().Where(s => supplierIds.Contains(s.Id)).ToListAsync();

        var modelById = models.ToDictionary(m => m.Id);
        var categoryById = categories.ToDictionary(c => c.Id);
        var manufacturerById = manufacturers.ToDictionary(m => m.Id);
        var statusById = statusLabels.ToDictionary(s => s.Id);
        var locationById = locations.ToDictionary(l => l.Id);
        var supplierById = suppliers.ToDictionary(s => s.Id);

        var result = new List<AssetWithDetails>(assets.Count);

        foreach (var asset in assets)
        {
            Model? model = null;
            Category? category = null;
            Manufacturer? manufacturer = null;
            StatusLabel? statusLabel = null;
            Location? location = null;
            Supplier? supplier = null;

            if (asset.ModelId.HasValue && modelById.TryGetValue((uint)asset.ModelId.Value, out var foundModel))
            {
                model = foundModel;

                if (model.CategoryId.HasValue)
                    categoryById.TryGetValue((uint)model.CategoryId.Value, out category);

                if (model.ManufacturerId.HasValue)
                    manufacturerById.TryGetValue((uint)model.ManufacturerId.Value, out manufacturer);
            }

            if (asset.StatusId.HasValue)
                statusById.TryGetValue((uint)asset.StatusId.Value, out statusLabel);

            if (asset.LocationId.HasValue)
                locationById.TryGetValue((uint)asset.LocationId.Value, out location);

            if (asset.SupplierId.HasValue)
                supplierById.TryGetValue((uint)asset.SupplierId.Value, out supplier);

            result.Add(new AssetWithDetails(asset, model, category, manufacturer, statusLabel, location, supplier));
        }

        return result;
    }

    public async Task<AssetWithDetails?> GetProductWithDetailsAsync(uint id)
    {
        var asset = await _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id && a.DeletedAt == null);

        if (asset == null)
            return null;

        var products = await ComposeAssetsWithDetailsAsync([asset]);
        return products[0];
    }

    public async Task<IEnumerable<AssetWithDetails>> GetAvailableProductsWithDetailsAsync()
    {
        var availabilityRules = await GetAvailabilityRulesAsync();

        var assets = await ApplyAvailableAssetFilter(_dbSet.AsNoTracking(), availabilityRules)
            .OrderBy(a => a.Name)
            .ToListAsync();
        return await ComposeAssetsWithDetailsAsync(assets);
    }

    public async Task<(IEnumerable<AssetWithDetails> Products, int TotalCount)> GetProductsPagedWithDetailsAsync(
        int pageNumber,
        int pageSize,
        int? categoryId = null,
        int? manufacturerId = null,
        int? statusId = null,
        string? searchTerm = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? hasPrice = null,
        bool? availableOnly = true)
    {
        var query = _dbSet.AsNoTracking().Where(a => a.DeletedAt == null);

        if (availableOnly == true)
        {
            var availabilityRules = await GetAvailabilityRulesAsync();
            query = ApplyAvailableAssetFilter(query, availabilityRules);
        }

        if (categoryId.HasValue)
        {
            var modelIds = await GetModelIdsByCategoryAsync(categoryId.Value);
            if (modelIds.Count == 0)
                return ([], 0);

            query = query.Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId));
        }

        if (manufacturerId.HasValue)
        {
            var modelIds = await GetModelIdsByManufacturerAsync(manufacturerId.Value);
            if (modelIds.Count == 0)
                return ([], 0);

            query = query.Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId));
        }

        if (statusId.HasValue)
            query = query.Where(a => a.StatusId == statusId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = $"%{searchTerm.Trim()}%";
            query = query.Where(a =>
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.AssetTag != null && EF.Functions.Like(a.AssetTag, term)) ||
                (a.Serial != null && EF.Functions.Like(a.Serial, term)));
        }

        if (minPrice.HasValue)
            query = query.Where(a => a.PurchaseCost >= minPrice.Value);
        if (maxPrice.HasValue)
            query = query.Where(a => a.PurchaseCost <= maxPrice.Value);

        if (hasPrice.HasValue)
        {
            query = hasPrice.Value
                ? query.Where(a => a.PurchaseCost.HasValue && a.PurchaseCost > 0)
                : query.Where(a => !a.PurchaseCost.HasValue || a.PurchaseCost <= 0);
        }

        var totalCount = await query.CountAsync();

        var assets = await query.OrderBy(a => a.Name).ThenBy(a => a.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var products = await ComposeAssetsWithDetailsAsync(assets);
        return (products, totalCount);
    }

    public async Task<IEnumerable<AssetWithDetails>> SearchProductsWithDetailsAsync(string searchTerm)
    {
        var term = $"%{searchTerm.Trim()}%";
        var assets = await _dbSet.AsNoTracking()
            .Where(a => a.DeletedAt == null && (
                (a.Name != null && EF.Functions.Like(a.Name, term)) ||
                (a.AssetTag != null && EF.Functions.Like(a.AssetTag, term)) ||
                (a.Serial != null && EF.Functions.Like(a.Serial, term))))
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .ToListAsync();

        return await ComposeAssetsWithDetailsAsync(assets);
    }

    public async Task<IEnumerable<AssetWithDetails>> GetByCategoryWithDetailsAsync(int categoryId)
    {
        var modelIds = await GetModelIdsByCategoryAsync(categoryId);
        if (modelIds.Count == 0)
            return [];

        var assets = await _dbSet.AsNoTracking()
            .Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId) &&
                a.DeletedAt == null)
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .ToListAsync();

        return await ComposeAssetsWithDetailsAsync(assets);
    }

    public async Task<IEnumerable<AssetWithDetails>> GetByManufacturerWithDetailsAsync(int manufacturerId)
    {
        var modelIds = await GetModelIdsByManufacturerAsync(manufacturerId);
        if (modelIds.Count == 0)
            return [];

        var assets = await _dbSet.AsNoTracking()
            .Where(a =>
                a.ModelId.HasValue &&
                modelIds.Contains(a.ModelId) &&
                a.DeletedAt == null)
            .OrderBy(a => a.Name)
            .ThenBy(a => a.Id)
            .ToListAsync();

        return await ComposeAssetsWithDetailsAsync(assets);
    }

    private async Task<List<int?>> GetModelIdsByCategoryAsync(int categoryId)
    {
        return await _context.Models.AsNoTracking()
            .Where(m => m.CategoryId == categoryId)
            .Select(m => (int?)m.Id)
            .ToListAsync();
    }

    private async Task<List<int?>> GetModelIdsByManufacturerAsync(int manufacturerId)
    {
        return await _context.Models.AsNoTracking()
            .Where(m => m.ManufacturerId == manufacturerId)
            .Select(m => (int?)m.Id)
            .ToListAsync();
    }

    private async Task<List<int>> GetActiveDeployableStatusIdsAsync()
    {
        return await _context.StatusLabels
            .AsNoTracking()
            .Where(s => s.Deployable && s.DeletedAt == null)
            .Select(s => (int)s.Id)
            .ToListAsync();
    }

    private async Task<List<int>> GetActiveRequestableModelIdsAsync()
    {
        return await _context.Models
            .AsNoTracking()
            .Where(m => m.Requestable == 1 && m.DeletedAt == null)
            .Select(m => (int)m.Id)
            .ToListAsync();
    }

    private async Task<(IReadOnlyCollection<int> DeployableStatusIds, IReadOnlyCollection<int> RequestableModelIds)> GetAvailabilityRulesAsync()
    {
        var deployableStatusIds = await GetActiveDeployableStatusIdsAsync();
        var requestableModelIds = await GetActiveRequestableModelIdsAsync();

        return (deployableStatusIds, requestableModelIds);
    }

    private static IQueryable<Asset> ApplyAvailableAssetFilter(
        IQueryable<Asset> query,
        (IReadOnlyCollection<int> DeployableStatusIds, IReadOnlyCollection<int> RequestableModelIds) rules)
    {
        if (rules.DeployableStatusIds.Count == 0 || rules.RequestableModelIds.Count == 0)
        {
            return query.Where(_ => false);
        }

        return query.Where(a =>
            a.StatusId.HasValue &&
            rules.DeployableStatusIds.Contains(a.StatusId.Value) &&
            a.ModelId.HasValue &&
            rules.RequestableModelIds.Contains(a.ModelId.Value) &&
            a.DeletedAt == null &&
            a.Requestable == 1 &&
            a.AssignedTo == null);
    }
}
