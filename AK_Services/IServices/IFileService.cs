namespace AK_Services.Interfaces;

public interface IFileService
{
    public Task<string> SaveFileAsync(IFormFile file , string? path = null);
    public Task DeleteFileAsync(string fileName);
    public Task EditFileAsync(IFormFile file, string path);
}