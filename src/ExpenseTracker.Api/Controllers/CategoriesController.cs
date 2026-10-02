using ExpenseTracker.Application.Categories;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController(ICategoryService categoryService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(CancellationToken ct)
        {
            return Ok(await categoryService.GetAllAsync(ct));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoryResponse>> GetById(Guid id, CancellationToken ct)
        {
            var category = await categoryService.GetIdAsync(id, ct);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> Create(CategoryRequest request, CancellationToken ct)
        {
            var category = await categoryService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);   
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoryResponse>> Update(Guid id, CategoryRequest request, CancellationToken ct)
        {
            var category = await categoryService.UpdateAsync(id, request, ct);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult>  Delete(Guid id, CancellationToken ct)
        {
            return await categoryService.DeleteAsync(id, ct) ? NoContent() : NotFound();
        }
    }
}
