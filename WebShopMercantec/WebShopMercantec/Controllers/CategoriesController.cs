using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShopMercantec.Shared.DTOs;
using WebShopMercantec.Services;
using FluentValidation;

namespace WebShopMercantec.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private readonly IValidator<CategoryDto> _validator;

    public CategoriesController(
        ICategoryService categoryService, 
        IValidator<CategoryDto> validator)
    {
        _categoryService = categoryService;
        _validator = validator;
    }

    // Get all categories
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // Get category by ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    // Get categories by type
    
    [HttpGet("type/{categoryType}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByType(string categoryType)
    {
        var categories = await _categoryService.GetCategoriesByTypeAsync(categoryType);
        return Ok(categories);
    }

    // Create a new category
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto categoryDto)
    {
        // Input validation
        var validationResult = await _validator.ValidateAsync(categoryDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var created = await _categoryService.CreateCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // Update a category
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryDto categoryDto)
    {
        // Input validation
        var validationResult = await _validator.ValidateAsync(categoryDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updated = await _categoryService.UpdateCategoryAsync(id, categoryDto);
        return Ok(updated);
    }

    // Delete a category (soft delete)
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }

    // Categories for the catalog storefront (assets only, honoring ShowInCatalog)
    
    [HttpGet("catalog")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCatalogCategories()
    {
        var categories = await _categoryService.GetCatalogCategoriesAsync(includeHidden: false);
        return Ok(categories);
    }

    // Categories for admin storefront settings (all asset categories)
    
    [HttpGet("catalog-admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCatalogCategoriesAdmin()
    {
        var categories = await _categoryService.GetCatalogCategoriesAsync(includeHidden: true);
        return Ok(categories);
    }

    // Enable/disable category visibility on the storefront
    
    [HttpPut("{id}/catalog-visibility")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> SetCatalogVisibility(int id, [FromBody] bool visible)
    {
        await _categoryService.SetCategoryVisibilityAsync(id, visible);
        return NoContent();
    }
}
