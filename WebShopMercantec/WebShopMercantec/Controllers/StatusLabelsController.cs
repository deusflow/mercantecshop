using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

/// <summary>
/// API контроллер для работы со статусами (StatusLabels)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class StatusLabelsController : ControllerBase
{
    private readonly IStatusLabelService _statusLabelService;

    public StatusLabelsController(IStatusLabelService statusLabelService)
    {
        _statusLabelService = statusLabelService;
    }

    /// <summary>
    /// Получить все статусы
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetAll()
    {
        var statusLabels = await _statusLabelService.GetAllStatusLabelsAsync();
        return Ok(statusLabels);
    }

    /// <summary>
    /// Получить статус по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StatusLabelDto>> GetById(int id)
    {
        var statusLabel = await _statusLabelService.GetStatusLabelByIdAsync(id);
        return Ok(statusLabel);
    }

    /// <summary>
    /// Получить статусы, доступные для deployment (можно выдавать пользователям)
    /// </summary>
    [HttpGet("deployable")]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetDeployable()
    {
        var statusLabels = await _statusLabelService.GetDeployableStatusesAsync();
        return Ok(statusLabels);
    }
}

