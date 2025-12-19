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
    private readonly ILogger<CategoriesController> _logger;
    private readonly IValidator<CategoryDto> _validator;

    public CategoriesController(
        ICategoryService categoryService, 
        ILogger<CategoriesController> logger,
        IValidator<CategoryDto> validator)
    {
        _categoryService = categoryService;
        _logger = logger;
        _validator = validator;
    }

    /// <summary>
    /// Получить все категории
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// Получить категорию по ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    /// <summary>
    /// Получить категории по типу
    /// </summary>
    [HttpGet("type/{categoryType}")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetByType(string categoryType)
    {
        var categories = await _categoryService.GetCategoriesByTypeAsync(categoryType);
        return Ok(categories);
    }

    /// <summary>
    /// Создать новую категорию
    /// Пример POST endpoint с валидацией через FluentValidation
    /// </summary>
    [HttpPost]
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

    /// <summary>
    /// Обновить категорию
    /// </summary>
    [HttpPut("{id}")]
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

    /// <summary>
    /// Удалить категорию (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}

