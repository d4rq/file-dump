namespace FileDump.Api.Core.Abstractions;

public interface IFileService
{
    public Task PutFileAsync(Stream file, string fileName, Guid userId, string contentType = "application/octet-stream");

    public Task<List<Core.Models.FileInfo>> GetFilesAsync(string prefix);

    public Task<Stream> GetFileAsync(string fileName);

    public Task DeleteFileAsync(string fileName);
}