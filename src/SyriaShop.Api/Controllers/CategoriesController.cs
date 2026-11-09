using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyriaShop.Application.Catalog;

namespace SyriaShop.Api.Controllers;

[ApiController]
[Route("api/categories")]
[Produces("application/json")]
public sealed class CategoriesController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CategoryDetailResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20,
        [FromQuery] bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var result = await categoryService.ListAsync(page, pageSize, includeInactive, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(CategoryDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await categoryService.GetBySlugAsync(slug, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CategoryCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await categoryService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(CategoryDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await categoryService.UpdateAsync(id, request, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await categoryService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
