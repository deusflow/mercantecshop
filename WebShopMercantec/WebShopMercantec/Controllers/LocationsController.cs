using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

// API контроллер для работы с локациями (Locations)

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    // Получить все локации
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    // Получить локацию по ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        return Ok(location);
    }
}

