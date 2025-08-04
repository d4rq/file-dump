namespace FileDump.Api.Data.Postgres.Entities;

public abstract class AuditableEntity : EntityBase
{
    public DateTime Created { get; set; }
    
    public DateTime Modified { get; set; }
}