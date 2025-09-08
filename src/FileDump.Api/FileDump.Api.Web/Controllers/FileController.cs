using FileDump.Api.Core.Abstractions;
using FileDump.Api.Data.Postgres;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FileDump.Api.Web.Controllers;

[ApiController]
[Route("api/File")]
[Authorize]
public class FileController : ControllerBase
{
    [HttpPost]
    public async Task PutFileAsync(IFormFile file, [FromServices] IFileService fileService)
    {
        var stream = new MemoryStream();
        
        await file.CopyToAsync(stream);

        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId")!.Value;
        
        await fileService.PutFileAsync(stream, file.FileName, Guid.Parse(userId), file.ContentType);
    }

    [HttpGet]
    public async Task<List<Core.Models.FileInfo>> GetFilesAsync([FromServices] IFileService fileService)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId")!.Value;
        return await fileService.GetFilesAsync($"user_{userId}/");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFileAsync(
        [FromRoute] Guid id,
        [FromServices] IFileService fileService,
        [FromServices] EfContext context)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId")!.Value;
        
        var file = await context.Files.FirstOrDefaultAsync(x => x.UserId.ToString() == userId && x.Id == id)
            ?? throw new Exception("File not found");

        var stream = await fileService.GetFileAsync(file.StorageName);
        stream.Position = 0;
        return File(stream, "application/octet-stream", file.Name);
    }

    [HttpDelete("{id}")]
    public async Task<bool> DeleteFileAsync(
        Guid id,
        [FromServices] IFileService fileService,
        [FromServices] EfContext context)
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "userId")!.Value;

        var file = await context.Files.FirstOrDefaultAsync(x => x.UserId.ToString() == userId && x.Id == id)
                   ?? throw new Exception("File not found");

        await fileService.DeleteFileAsync(file.StorageName);
        
        context.Files.Remove(file);
        return await context.SaveChangesAsync() == 1;
    }
}