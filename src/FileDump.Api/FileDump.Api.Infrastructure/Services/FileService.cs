using FileDump.Api.Core.Abstractions;
using FileDump.Api.Data.Postgres;
using Microsoft.EntityFrameworkCore;
using File = FileDump.Api.Data.Postgres.Entities.File;

namespace FileDump.Api.Infrastructure.Services;

public class FileService(IMinioService minioService, EfContext context) : IFileService
{
    public async Task PutFileAsync(Stream file, string fileName, Guid userId, string contentType = "application/octet-stream")
    {
        var id = Guid.NewGuid();
        var storageName = $"user_{userId}/{id}_{fileName}";
        var fileForDb = new File { Id = id, Name = fileName, StorageName = storageName, UserId = userId };

        if (await minioService.PutFileAsync(file, storageName, contentType))
        {
            await context.Files.AddAsync(fileForDb);
            await context.SaveChangesAsync();
        }
    }

    public async Task<List<Core.Models.FileInfo>> GetFilesAsync(string prefix)
    {
        var objects = await minioService.GetFilesAsync(prefix);
        return objects;
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        var file = await minioService.GetFileAsync(fileName);
        return file;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        await minioService.DeleteFileAsync(fileName);
    }
}