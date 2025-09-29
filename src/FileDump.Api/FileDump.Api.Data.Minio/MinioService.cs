using System.Net;
using FileDump.Api.Core.Abstractions;
using Minio;
using Minio.DataModel.Args;

namespace FileDump.Api.Data.Minio;

public class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    
    public MinioService(IMinioClient client)
    {
        _client = client;
    }

    private async Task EnsureBucketExistsAsync()
    {
        var bucketExistsArgs = new BucketExistsArgs()
            .WithBucket("file-dump");
        
        var bucketExists = await _client.BucketExistsAsync(bucketExistsArgs);

        if (!bucketExists)
        {
            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket("file-dump");

            await _client.MakeBucketAsync(makeBucketArgs);
        }
    }

    public async Task<bool> PutFileAsync(Stream fileStream, string fileName, string contentType = "application/octet-stream")
    {
        await EnsureBucketExistsAsync();
        
        if (fileStream.CanSeek)
            fileStream.Position = 0;

        var putObjectArgs = new PutObjectArgs()
            .WithBucket("file-dump")
            .WithObject(fileName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);
        
        var response = await _client.PutObjectAsync(putObjectArgs);
        
        return response.ResponseStatusCode == HttpStatusCode.OK;
    }

    public async Task<List<Core.Models.FileInfo>> GetFilesAsync(string prefix)
    {
        await EnsureBucketExistsAsync();
        
        var listObjectsArgs = new ListObjectsArgs()
            .WithBucket("file-dump")
            .WithPrefix(prefix);
        
        var items = _client.ListObjectsEnumAsync(listObjectsArgs);

        var result = new List<Core.Models.FileInfo>();
        
        await foreach (var item in items)
            result.Add(new Core.Models.FileInfo
            {
                LastModified = FormatDateTime(item.LastModifiedDateTime!.Value),
                Size = ConvertSizeToString(item.Size),
                Name = new string(item.Key.Split("/").Last().Skip(37).ToArray()),
                Id = Guid.Parse(item.Key.Split("/").Last().Split("_").First())
            });
        
        return result;
    }

    public async Task<Stream> GetFileAsync(string fileName)
    {
        await EnsureBucketExistsAsync();
        
        var memoryStream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket("file-dump")
            .WithObject(fileName)
            .WithCallbackStream(stream =>
            {
                stream.CopyTo(memoryStream);
            });
        
        await _client.GetObjectAsync(getObjectArgs);
        
        return memoryStream;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        await EnsureBucketExistsAsync();
        
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket("file-dump")
            .WithObject(fileName);
        
        await _client.RemoveObjectAsync(removeObjectArgs);
    }

    private string ConvertSizeToString(ulong size)
    {
        string[] sizeSuffixes = 
            { "B", "KiB", "MiB", "GiB", "TiB", "PiB", "EiB" };
        
        int place = Convert.ToInt32(Math.Floor(Math.Log(size, 1024)));
        
        // Ограничиваем максимальный порядок
        if (place >= sizeSuffixes.Length) 
            place = sizeSuffixes.Length - 1;
        
        // Вычисляем размер в выбранных единицах
        double num = Math.Round(size / Math.Pow(1024, place), 1);
        
        return $"{num} {sizeSuffixes[place]}";
    }

    private string FormatDateTime(DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy HH:mm");
    }
}