using System;
using System.IO.Compression;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json.Serialization;
using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
ConfigureAuthentication(builder);
ConfigureMvc(builder);
ConfigureServices(builder);

//Documentacao com Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
LoadConfiguration(app);
app.UseHttpsRedirection();
app.UseAuthentication();//autenticação primeiro que autorização
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles(); //para renderizar imagens, js e css, vai procurar em wwwroot
app.UseResponseCompression();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Ambiente: Dev");
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.Run();

void LoadConfiguration(WebApplication app)
{
    // Passando informacoes o appsettings para as propriedades da classe Configuration
    Configuration.JwtKey = app.Configuration.GetValue<string>("JwtKey");
    Configuration.ApiKeyName = app.Configuration.GetValue<string>("ApiKeyName");
    Configuration.ApiKeyValue = app.Configuration.GetValue<string>("ApiKeyValue");
    Configuration.AppPort = app.Configuration.GetValue<int>("AppPort");

    var smtp = new Configuration.SmtpConfiguration();
    app.Configuration.GetSection("Smtp").Bind(smtp);
    Configuration.Smtp = smtp;
}
void ConfigureAuthentication(WebApplicationBuilder builder)
{
    // Usando a chave jwt para encriptar e desencriptar token
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
}
void ConfigureMvc(WebApplicationBuilder builder)
{
    builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true; /*Remover validacao automatica do payload da requisicao*/
    })
    //Resolvendo problemas de serialização, ciclos subsequentes, vai até primeiro nó
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;//objetos que estiverem como null serão ignorados
    });
}
void ConfigureServices(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<BlogDataContext>(options => options.UseSqlServer(connectionString));
    builder.Services.AddTransient<TokenService>();
    builder.Services.AddMemoryCache();
    builder.Services.AddResponseCompression(opt =>
    {
        opt.Providers.Add<GzipCompressionProvider>();
    });
    builder.Services.Configure<GzipCompressionProviderOptions>(opt =>
    {
        opt.Level = CompressionLevel.Optimal;
    });
}