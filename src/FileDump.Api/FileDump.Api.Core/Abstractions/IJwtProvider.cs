namespace FileDump.Api.Core.Abstractions;

public interface IJwtProvider
{
    public string GenerateToken(Guid userId);
}