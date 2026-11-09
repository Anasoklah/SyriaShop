using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyriaShop.Application.Catalog;

namespace SyriaShop.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductListItemResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ProductListQuery query, CancellationToken cancellationToken = default)
    {
        var result = await productService.ListAsync(query, cancellationToken);

        return Ok(result);
    }

    [HttpGet("{slug}")]
    [ProducesResponseType(typeof(ProductDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var result = await productService.GetBySlugAsync(slug, cancellationToken);

        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] ProductCreateRequest request, CancellationToken cancellationToken)
    {
        var result = await productService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetBySlug), new { slug = result.Slug }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductUpdateRequest request, CancellationToken cancellationToken)
    {
        var result = await productService.UpdateAsync(id, request, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
