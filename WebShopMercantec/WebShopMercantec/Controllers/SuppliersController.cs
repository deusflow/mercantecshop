using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SuppliersController : ControllerBase
{
    private readonly SnipeItContext _context;

    public SuppliersController(SnipeItContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var suppliers = await _context.Suppliers
            .AsQueryable()
            .Where(s => s.DeletedAt == null)
            .OrderBy(s => s.Name)
            .Select(s => new SupplierDto
            {
                Id = (int)s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                State = s.State,
                Country = s.Country,
                Zip = s.Zip,
                Contact = s.Contact,
                Phone = s.Phone,
                Fax = s.Fax,
                Email = s.Email,
                Url = s.Url,
                Notes = s.Notes
            })
            .ToListAsync();

        return Ok(suppliers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id)
    {
        var supplier = await _context.Suppliers
            .AsQueryable()
            .Where(s => s.Id == id && s.DeletedAt == null)
            .Select(s => new SupplierDto
            {
                Id = (int)s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                State = s.State,
                Country = s.Country,
                Zip = s.Zip,
                Contact = s.Contact,
                Phone = s.Phone,
                Fax = s.Fax,
                Email = s.Email,
                Url = s.Url,
                Notes = s.Notes
            })
            .FirstOrDefaultAsync();

        if (supplier == null)
            throw new NotFoundException("Supplier", id);

        return Ok(supplier);
    }
}

