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

    // Получить все категории
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    // Получить категорию по ID
    
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    // Получить категории по типу
    
    [HttpGet("type/{categoryType}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByType(string categoryType)
    {
        var categories = await _categoryService.GetCategoriesByTypeAsync(categoryType);
        return Ok(categories);
    }

    // Создать новую категорию
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CategoryDto categoryDto)
    {
        // Валидация входных данных
        var validationResult = await _validator.ValidateAsync(categoryDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var created = await _categoryService.CreateCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // Обновить категорию
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] CategoryDto categoryDto)
    {
        // Валидация входных данных
        var validationResult = await _validator.ValidateAsync(categoryDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updated = await _categoryService.UpdateCategoryAsync(id, categoryDto);
        return Ok(updated);
    }

    // Удалить категорию (soft delete)
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }

    // Категории для витрины каталога (только asset, с учетом ShowInCatalog)
    
    [HttpGet("catalog")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCatalogCategories()
    {
        var categories = await _categoryService.GetCatalogCategoriesAsync(includeHidden: false);
        return Ok(categories);
    }

    // Категории для админ-настроек витрины (все asset категории)
    
    [HttpGet("catalog-admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCatalogCategoriesAdmin()
    {
        var categories = await _categoryService.GetCatalogCategoriesAsync(includeHidden: true);
        return Ok(categories);
    }

    // Включить/выключить отображение категории на главной витрине
    
    [HttpPut("{id}/catalog-visibility")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult> SetCatalogVisibility(int id, [FromBody] bool visible)
    {
        await _categoryService.SetCategoryVisibilityAsync(id, visible);
        return NoContent();
    }
}
