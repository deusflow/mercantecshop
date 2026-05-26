using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Repositories;
using WebShopMercantec.Exceptions;
using WebShopMercantec.Mapping;

namespace WebShopMercantec.Services;

public class StatusLabelService : IStatusLabelService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatusLabelService> _logger;

    public StatusLabelService(IUnitOfWork unitOfWork, ILogger<StatusLabelService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IEnumerable<StatusLabelDto>> GetAllStatusLabelsAsync()
    {
        _logger.LogInformation("Getting all status labels");
        var statusLabels = await _unitOfWork.StatusLabels.GetAllActiveAsync();
        
        var statusLabelDtos = new List<StatusLabelDto>();
        foreach (var statusLabel in statusLabels)
        {
            var assetsCount = await _unitOfWork.StatusLabels.GetAssetsCountAsync(statusLabel.Id);
            statusLabelDtos.Add(StatusLabelMapping.MapToDto(statusLabel, assetsCount));
        }
        
        _logger.LogInformation("Found {Count} status labels", statusLabelDtos.Count);
        return statusLabelDtos;
    }

    public async Task<StatusLabelDto?> GetStatusLabelByIdAsync(int id)
    {
        _logger.LogInformation("Getting status label with ID: {StatusLabelId}", id);
        var statusLabel = await _unitOfWork.StatusLabels.GetActiveByIdAsync((uint)id);
        
        if (statusLabel == null)
        {
            _logger.LogWarning("Status label not found: {StatusLabelId}", id);
            throw new NotFoundException("StatusLabel", id);
        }
        
        var assetsCount = await _unitOfWork.StatusLabels.GetAssetsCountAsync(statusLabel.Id);
        return StatusLabelMapping.MapToDto(statusLabel, assetsCount);
    }

    public async Task<IEnumerable<StatusLabelDto>> GetDeployableStatusesAsync()
    {
        _logger.LogInformation("Getting deployable status labels");
        var statusLabels = await _unitOfWork.StatusLabels.GetDeployableStatusesAsync();
        var statusLabelDtos = StatusLabelMapping.MapToDtos(statusLabels).ToList();
        _logger.LogInformation("Found {Count} deployable status labels", statusLabelDtos.Count);
        return statusLabelDtos;
    }
}
