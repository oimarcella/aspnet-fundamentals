using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
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
        [FromQuery] int pageSize = 10
    )
    {
        try
        {
            var count = await context.Posts.AsNoTracking().CountAsync();
            var posts = await context.Posts
            .AsNoTracking()
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
            .OrderByDescending(x => x.LastUpdateDate)
            .Skip(page * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

            return Ok(new ResultViewModel<dynamic>(new
            {
                total = count,
                page,
                pageSize,
                posts
            }));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(ResultViewModel<string>.StatusCode.InternalServerError));
        }
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> DetailsAsync(
        [FromServices] BlogDataContext context,
        [FromRoute] int id
    )
    {
        try
        {
            var post = await context
            .Posts
            .AsNoTracking()
            .Include(x => x.Author)
            .ThenInclude(c => c.Roles) //roles do author
            .Include(x => x.Category)
            .FirstOrDefaultAsync(x => x.Id == id);
            if (post == null) return StatusCode(204, new ResultViewModel<Post>("Nada encontrado na busca", isError: false));
            return Ok(new ResultViewModel<Post>(post));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(ResultViewModel<string>.StatusCode.InternalServerError));
        }
    }
}