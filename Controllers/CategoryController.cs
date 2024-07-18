using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [ApiController]
    public class CategoryController : Controller
    {
        [HttpGet("v1/hello")]
        public IActionResult Hello([FromServices] BlogDataContext context)
        {
            return Ok(new {message = "Hello! I'm here."});
        }

        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
        {
            try
            {
                var categories = await context.Categories.ToListAsync();
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id
        )
        {
            try{
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

            if (category == null) return NotFound(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.NotFound));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
        }

        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync (
            [FromServices] BlogDataContext context,
            [FromBody] EditorCategoryViewModel model
        )
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.BadRequest));

            try
            {
                var newCategory = new Category {
                    Id = 0,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                    Posts = []
                };

                await context.Categories.AddAsync(newCategory);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{newCategory.Id}", new ResultViewModel<Category>(newCategory));
            }
            catch(DbUpdateException) {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
            catch (Exception) {
                return StatusCode(500, ResultViewModel<Category>.StatusCode.InternalServerError);
            }
        }

        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model
        )
        {
            if (!ModelState.IsValid) return BadRequest(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.BadRequest));

            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);

                if (category == null) return NotFound(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.NotFound));

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromServices] BlogDataContext context,
            [FromRoute] int id
        ) {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null) return NotFound(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.NotFound));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.Ok));
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
            catch (Exception)
            {
                return StatusCode(500, new ResultViewModel<Category>(ResultViewModel<Category>.StatusCode.InternalServerError));
            }
        }
    }
}
