namespace FileDump.Api.Core.Abstractions;

public interface IMinioService
{
    public Task<bool> PutFileAsync(Stream fileStream, string fileName, string contentType = "application/octet-stream");

    public Task<List<Core.Models.FileInfo>> GetFilesAsync(string prefix);

    public Task<Stream> GetFileAsync(string fileName);

    public Task DeleteFileAsync(string fileName);
}