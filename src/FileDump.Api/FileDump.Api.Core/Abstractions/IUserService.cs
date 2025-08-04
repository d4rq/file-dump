namespace FileDump.Api.Core.Abstractions;

public interface IUserService
{
    public Task Register(string username, string email, string password, CancellationToken cancellationToken = default);

    
    public Task<string> Login(string username, string password, CancellationToken cancellationToken = default);
}