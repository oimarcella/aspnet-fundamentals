using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; /*Remover validação automática do payload da requisição*/
});
builder.Services.AddDbContext<BlogDataContext>();
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

app.MapControllers();

app.Run();
