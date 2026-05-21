using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

// API controller for status labels

[Route("api/[controller]")]
[ApiController]
public class StatusLabelsController : ControllerBase
{
    private readonly IStatusLabelService _statusLabelService;

    public StatusLabelsController(IStatusLabelService statusLabelService)
    {
        _statusLabelService = statusLabelService;
    }

    // Get all status labels
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetAll()
    {
        var statusLabels = await _statusLabelService.GetAllStatusLabelsAsync();
        return Ok(statusLabels);
    }

    // Get status label by ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StatusLabelDto>> GetById(int id)
    {
        var statusLabel = await _statusLabelService.GetStatusLabelByIdAsync(id);
        return Ok(statusLabel);
    }

    // Get status labels available for deployment (assignable to users)
    
    [HttpGet("deployable")]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetDeployable()
    {
        var statusLabels = await _statusLabelService.GetDeployableStatusesAsync();
        return Ok(statusLabels);
    }
}
