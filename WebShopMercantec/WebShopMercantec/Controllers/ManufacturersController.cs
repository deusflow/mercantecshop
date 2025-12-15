using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ManufacturersController : ControllerBase
{
    private readonly SnipeItContext _context;

    public ManufacturersController(SnipeItContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ManufacturerDto>>> GetAll()
    {
        var manufacturers = await _context.Manufacturers
            .AsQueryable()
            .Where(m => m.DeletedAt == null)
            .OrderBy(m => m.Name)
            .Select(m => new ManufacturerDto
            {
                Id = (int)m.Id,
                Name = m.Name,
                Url = m.Url,
                SupportUrl = m.SupportUrl,
                SupportPhone = m.SupportPhone,
                SupportEmail = m.SupportEmail,
                ProductsCount = 0 // TODO: Calculate from database
            })
            .ToListAsync();

        return Ok(manufacturers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ManufacturerDto>> GetById(int id)
    {
        var manufacturer = await _context.Manufacturers
            .AsQueryable()
            .Where(m => m.Id == id && m.DeletedAt == null)
            .Select(m => new ManufacturerDto
            {
                Id = (int)m.Id,
                Name = m.Name,
                Url = m.Url,
                SupportUrl = m.SupportUrl,
                SupportPhone = m.SupportPhone,
                SupportEmail = m.SupportEmail,
                ProductsCount = 0 // TODO: Calculate from database
            })
            .FirstOrDefaultAsync();

        if (manufacturer == null)
            throw new NotFoundException("Manufacturer", id);

        return Ok(manufacturer);
    }
}

