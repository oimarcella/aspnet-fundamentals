using System.Text;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true; /*Remover valida��o autom�tica do payload da requisi��o*/
});
builder.Services.AddDbContext<BlogDataContext>();
builder.Services.AddTransient<TokenService>();

var app = builder.Build();

// Passando informacoes o appsettings para as propriedades da classe Configuration
Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
Configuration.ApiKeyValue = app.Configuration.GetValue<string>("ApiKeyValue");
var smtp = new Configuration.SmtpConfiguration();
app.Configuration.GetSection("Smtp").Bind(smtp);
Configuration.Smtp = smtp;

app.MapControllers();
app.Run();
