using FileDump.Api.Core.Abstractions;

namespace FileDump.Api.Infrastructure.Utilities;

public class PasswordHasher : IPasswordHasher
{
    public string Generate(string password)
        => BCrypt.Net.BCrypt.EnhancedHashPassword(password);

    public bool Verify(string password, string hash)
        => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
}