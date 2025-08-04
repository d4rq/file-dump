using FileDump.Api.Contracts.Requests.Auth;
using FileDump.Api.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace FileDump.Api.Web.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("register")]
    public async Task Register(
        [FromServices] IUserService userService,
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        await userService.Register(request.Username, request.Email, request.Password, cancellationToken);
    }

    [HttpPost("login")]
    public async Task<string> Login(
        [FromServices] IUserService userService,
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var token = await userService.Login(request.Username, request.Password, cancellationToken);
        
        HttpContext.Response.Cookies.Append("auth", token);

        return token;
    }

    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        HttpContext.Response.Cookies.Delete("auth");
        return Task.FromResult<IActionResult>(NoContent());
    }
}