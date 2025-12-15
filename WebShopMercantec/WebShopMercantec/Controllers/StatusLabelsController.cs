using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebShopMercantec.Models;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Exceptions;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StatusLabelsController : ControllerBase
{
    private readonly SnipeItContext _context;

    public StatusLabelsController(SnipeItContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StatusLabelDto>>> GetAll()
    {
        var statusLabels = await _context.StatusLabels
            .AsQueryable()
            .Where(s => s.DeletedAt == null)
            .OrderBy(s => s.Name)
            .Select(s => new StatusLabelDto
            {
                Id = (int)s.Id,
                Name = s.Name ?? "Unknown",
                Color = s.Color,
                Deployable = s.Deployable,
                Pending = s.Pending,
                Archived = s.Archived,
                Notes = s.Notes,
                ShowInNav = s.ShowInNav,
                DefaultLabel = s.DefaultLabel,
                AssetsCount = 0 // TODO: Calculate from database
            })
            .ToListAsync();

        return Ok(statusLabels);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StatusLabelDto>> GetById(int id)
    {
        var statusLabel = await _context.StatusLabels
            .AsQueryable()
            .Where(s => s.Id == id && s.DeletedAt == null)
            .Select(s => new StatusLabelDto
            {
                Id = (int)s.Id,
                Name = s.Name ?? "Unknown",
                Color = s.Color,
                Deployable = s.Deployable,
                Pending = s.Pending,
                Archived = s.Archived,
                Notes = s.Notes,
                ShowInNav = s.ShowInNav,
                DefaultLabel = s.DefaultLabel,
                AssetsCount = 0 // TODO: Calculate from database
            })
            .FirstOrDefaultAsync();

        if (statusLabel == null)
            throw new NotFoundException("StatusLabel", id);

        return Ok(statusLabel);
    }
}

