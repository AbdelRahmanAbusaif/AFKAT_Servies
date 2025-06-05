namespace AK_Services.Interfaces;

public interface IFileService
{
    public Task<string> SaveFileAsync(IFormFile file);
    public Task DeleteFileAsync(string fileName);
    public Task EditFileAsync(IFormFile file, string path);
}