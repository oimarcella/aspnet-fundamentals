using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAsync(
        [FromServices] BlogDataContext context,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 0
    )
    {
        var posts = await context.Posts
        .Include(c => c.Category)
        .Include(x => x.Author)
        .Select(x => new ListPostsViewModel
        {
            Id = x.Id,
            Title = x.Title,
            Slug = x.Slug,
            LastUpdateDate = x.LastUpdateDate,
            Category = x.Category.Name,
            Author = $"{x.Author.Name} ({x.Author.Email})"
        })
        .Skip(page * pageSize)
        .Take(pageSize)
        .ToArrayAsync();

        return Ok(posts);
    }
}