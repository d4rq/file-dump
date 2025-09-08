namespace FileDump.Api.Data.Postgres.Entities;

public class File : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    
    public string StorageName { get; set; } = string.Empty;
    
    public Guid UserId { get; set; }
    
    public User? User { get; set; }
}