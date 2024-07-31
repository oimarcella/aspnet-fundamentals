using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;
public class AccountController : ControllerBase
{

    [HttpPost("v1/login")]
    public IActionResult Login([FromServices] TokenService tokenService)
    {
        var token = tokenService.GenerateToken(null);
        return Ok(new ResultViewModel<string>(token, isError: false));
    }

    [Authorize(Roles = "user")]
    [Route("v1/user")]
    public IActionResult GetUser() => Ok(User.Identity.Name);

    [Authorize(Roles = "author")]
    [Authorize(Roles = "admin")]
    [Authorize]
    public IActionResult GetAuthor() => Ok(User.Identity.Name);

    [Authorize(Roles = "admin")]
    [Authorize]
    public IActionResult GetAdmin() => Ok(User.Identity.Name);
}
