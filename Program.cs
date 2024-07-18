using Blog.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; /*Remover valida��o autom�tica do payload da requisi��o*/
});
builder.Services.AddDbContext<BlogDataContext>();
var app = builder.Build();

app.MapControllers();

app.Run();
