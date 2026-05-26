using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;

namespace WebShopMercantec.Controllers;

// API controller for manufacturers

[Route("api/[controller]")]
[ApiController]
public class ManufacturersController : ControllerBase
{
    private readonly IManufacturerService _manufacturerService;

    public ManufacturersController(IManufacturerService manufacturerService)
    {
        _manufacturerService = manufacturerService;
    }

    // Get all manufacturers
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetAll()
    {
        var manufacturers = await _manufacturerService.GetAllManufacturersAsync();
        return Ok(manufacturers);
    }

    // Get manufacturer by ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ManufacturerDto>> GetById(int id)
    {
        var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(id);
        return Ok(manufacturer);
    }
}
