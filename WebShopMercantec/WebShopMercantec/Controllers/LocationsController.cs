using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly SnipeItContext _context;

    public LocationsController(SnipeItContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetAll()
    {
        var locations = await _context.Locations
            .AsQueryable()
            .Where(l => l.DeletedAt == null)
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto
            {
                Id = (int)l.Id,
                Name = l.Name ?? "Unknown",
                Address = l.Address,
                Address2 = l.Address2,
                City = l.City,
                State = l.State,
                Country = l.Country,
                Zip = l.Zip
            })
            .ToListAsync();

        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetById(int id)
    {
        var location = await _context.Locations
            .AsQueryable()
            .Where(l => l.Id == id && l.DeletedAt == null)
            .Select(l => new LocationDto
            {
                Id = (int)l.Id,
                Name = l.Name ?? "Unknown",
                Address = l.Address,
                Address2 = l.Address2,
                City = l.City,
                State = l.State,
                Country = l.Country,
                Zip = l.Zip
            })
            .FirstOrDefaultAsync();

        if (location == null)
            throw new NotFoundException("Location", id);

        return Ok(location);
    }
}

