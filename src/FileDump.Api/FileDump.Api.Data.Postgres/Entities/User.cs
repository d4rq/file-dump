namespace FileDump.Api.Data.Postgres.Entities;

public class User : AuditableEntity
{
    public User(string email, string username, string password)
    {
        Email = email;
        Username = username;
        Password = password;
    }
    
    public string Email { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
}