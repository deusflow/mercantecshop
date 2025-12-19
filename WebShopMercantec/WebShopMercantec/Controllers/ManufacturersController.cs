using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

/// <summary>
/// API контроллер для работы с производителями (Manufacturers)
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class ManufacturersController : ControllerBase
{
    private readonly IManufacturerService _manufacturerService;

    public ManufacturersController(IManufacturerService manufacturerService)
    {
        _manufacturerService = manufacturerService;
    }

    /// <summary>
    /// Получить всех производителей
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetAll()
    {
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
        return Ok(manufacturers);
    }

    /// <summary>
    /// Получить производителя по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ManufacturerDto>> GetById(int id)
    {
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
        return Ok(manufacturer);
    }
}

