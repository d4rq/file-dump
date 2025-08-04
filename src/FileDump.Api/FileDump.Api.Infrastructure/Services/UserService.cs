using FileDump.Api.Core.Abstractions;
using FileDump.Api.Data.Postgres;
using FileDump.Api.Data.Postgres.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileDump.Api.Infrastructure.Services;

public class UserService(IPasswordHasher passwordHasher, EfContext context, IJwtProvider jwtProvider) : IUserService
{
    public async Task Register(
        string username,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var hash = passwordHasher.Generate(password);

        var user = new User(email, username, hash);

        await context.Users.AddAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<string> Login(
        string username,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await context.Users
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Username == username, cancellationToken)
        ?? throw new Exception();

        var result = passwordHasher.Verify(password, user.Password);

        if (result == false)
            throw new Exception("Failed to login");

        var token = jwtProvider.GenerateToken(user.Id);

        return token;
    }
}