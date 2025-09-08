namespace FileDump.Api.Contracts.Requests.File;

public class PutFileRequest
{
    public Stream File { get; set; }
    
    public string FileName { get; set; }
}