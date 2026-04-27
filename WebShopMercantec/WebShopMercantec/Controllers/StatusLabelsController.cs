using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

// API контроллер для работы со статусами (StatusLabels)

[Route("api/[controller]")]
[ApiController]
public class StatusLabelsController : ControllerBase
{
    private readonly IStatusLabelService _statusLabelService;

    public StatusLabelsController(IStatusLabelService statusLabelService)
    {
        _statusLabelService = statusLabelService;
    }

    // Получить все статусы
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetAll()
    {
        var statusLabels = await _statusLabelService.GetAllStatusLabelsAsync();
        return Ok(statusLabels);
    }

    // Получить статус по ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<StatusLabelDto>> GetById(int id)
    {
        var statusLabel = await _statusLabelService.GetStatusLabelByIdAsync(id);
        return Ok(statusLabel);
    }

    // Получить статусы, доступные для deployment (можно выдавать пользователям)
    
    [HttpGet("deployable")]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetDeployable()
    {
        var statusLabels = await _statusLabelService.GetDeployableStatusesAsync();
        return Ok(statusLabels);
    }
}

