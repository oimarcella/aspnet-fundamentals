using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers;
public class AccountController : ControllerBase
{
    private readonly TokenService _tokenService;
    public AccountController(TokenService tokenService) {
        _tokenService = tokenService;
    }
    [HttpPost("v1/login")]
    public IActionResult Login() {
        var token = _tokenService.GenerateToken(null);
        return Ok(new ResultViewModel<TokenService>(token));
    }
}
