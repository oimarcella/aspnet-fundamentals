using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blog.Controllers;
public class AccountController : ControllerBase
{
    [HttpPost("v1/accounts/")]
    public async Task<IActionResult> Post(
        [FromBody] RegisterViewModel model,
        [FromServices] BlogDataContext context
    )
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ResultViewModel<string>.StatusCode.BadRequest));
        var user = new User
        {
            Name = model.Name,
            Email = model.Email,
            Slug = model.Email.Replace("@", "-").Replace(".", "-"),
        };

        var password = PasswordGenerator.Generate(length: 25, includeSpecialChars: true, upperCase: false);
        user.PasswordHash = PasswordHasher.Hash(password);

        try
        {
            await context.AddAsync(user);
            await context.SaveChangesAsync();
            //retornando a senha para tela somente para teste --relax
            var response = new
            {
                user = user.Email,
                password
            };

            // Retorne o resultado
            return Ok(new ResultViewModel<dynamic>(response));
        }
        catch (DbUpdateException error)
        {
            Console.WriteLine(error);
            return StatusCode(400, new ResultViewModel<dynamic>("E-mail já existe"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(
             ResultViewModel<dynamic>.StatusCode.InternalServerError
            ));
        }
    }

    [HttpPost("v1/accounts/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginViewModel model,
        [FromServices] TokenService tokenService,
        [FromServices] BlogDataContext context)
    {
        if (!ModelState.IsValid) return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));
        var user = await context.Users
               .AsNoTracking()
               .Include(x => x.Roles)
               .FirstOrDefaultAsync(x => x.Email == model.Email);

        if (user == null)
            return StatusCode(401, new ResultViewModel<dynamic>("E-mail ou senha inválidos"));

        if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
            return StatusCode(401, new ResultViewModel<dynamic>("E-mail ou senha inválidos"));

        try
        {
            var token = tokenService.GenerateToken(user);
            return Ok(new ResultViewModel<string>(token, errors: null));
        }
        catch (DbUpdateException error)
        {
            Console.WriteLine(error);
            return StatusCode(400, new ResultViewModel<dynamic>("Usuário não encontrado"));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(
             ResultViewModel<dynamic>.StatusCode.InternalServerError
            ));
        }
    }


    [Authorize]
    [HttpPost("v1/accounts/upload-image")]
    public async Task<IActionResult> UploadImage(
        [FromBody] UploadImageViewModel model,
        [FromServices] BlogDataContext context
    )
    {
        var filename = $"{Guid.NewGuid()}.jpg";
        var data = new Regex(@"^data:image\/[a-z]+;base64,").Replace(model.Base64Image, "");
        var bytes = Convert.FromBase64String(data);

        try
        {
            await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{filename}", bytes);
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(ResultViewModel<dynamic>.StatusCode.InternalServerError));
        }

        var user = await context
        .Users
        .FirstOrDefaultAsync(x => x.Email == User.Identity.Name);

        if (user == null)
            return NotFound(new ResultViewModel<dynamic>("Usuário não enconrado"));

        user.Image = $"http://localhost:{Configuration.AppPort}/images/{filename}";

        try
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<dynamic>(ResultViewModel<dynamic>.StatusCode.InternalServerError));
        }

        return Ok(new ResultViewModel<dynamic>(new { user.Image }));
    }
}
