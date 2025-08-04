namespace FileDump.Api.Infrastructure.Utilities;

public class JwtOptions
{
    public string SecretKey { get; set; }
    
    public int ExpiresHours { get; set; }
}