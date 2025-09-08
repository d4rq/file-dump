using System.Text;
using FileDump.Api.Core.Abstractions;
using FileDump.Api.Data.Minio;
using FileDump.Api.Data.Postgres;
using FileDump.Api.Infrastructure.Services;
using FileDump.Api.Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers();
services.AddSwaggerGen();

services.AddDbContext<EfContext>(
    options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IUserService, UserService>();

services.Configure<MinioOptions>(configuration.GetSection("MinioOptions"));
services.AddSingleton<IMinioClient>(provider =>
{
    var options = provider.GetService<IOptions<MinioOptions>>()!.Value;
    
    return new MinioClient()
        .WithEndpoint(new Uri(options.Endpoint))
        .WithCredentials(options.AccessKey, options.SecretKey)
        .Build();
});
services.AddScoped<IMinioService, MinioService>();

services.AddScoped<IFileService, FileService>();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
    {
        opts.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration.GetValue<string>("JwtOptions:SecretKey")!))
        };

        opts.Events = new JwtBearerEvents()
        {
            OnMessageReceived = context =>
            {
                context.Token = context.Request.Cookies["auth"];
                return Task.CompletedTask;
            }
        };
    });
services.AddAuthorization();

var app = builder.Build();

app.MapControllers();

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();