using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly SnipeItContext _context;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(SnipeItContext context, ILogger<CategoriesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _context.Categories
            .AsQueryable()
            .Where(c => c.DeletedAt == null)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto
            {
                Id = (int)c.Id,
                Name = c.Name,
                CategoryType = c.CategoryType,
                ItemsCount = 0, // TODO: Calculate actual count
                Image = c.Image
            })
            .ToListAsync();

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _context.Categories
            .AsQueryable()
            .Where(c => c.Id == id && c.DeletedAt == null)
            .Select(c => new CategoryDto
            {
                Id = (int)c.Id,
                Name = c.Name,
                CategoryType = c.CategoryType,
                ItemsCount = 0, // TODO: Calculate actual count
                Image = c.Image
            })
            .FirstOrDefaultAsync();

        if (category == null)
            throw new NotFoundException("Category", id);

        return Ok(category);
    }
}

