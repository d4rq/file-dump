namespace FileDump.Api.Core.Models;

public class FileInfo
{
    public string LastModified { get; set; }
    
    public string Size { get; set; }
    
    public string Name { get; set; }
    
    public Guid Id { get; set; }
}