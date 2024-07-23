using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;
public class AccountController : ControllerBase
{
    [HttpPost("v1/login")]
    public IActionResult Login([FromServices] TokenService tokenService) {
        var token = tokenService.GenerateToken(null);
        return Ok(new ResultViewModel<TokenService>(token));
    }
}
