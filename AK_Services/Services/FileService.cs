using AK_Services.Interfaces;
using Azure.Storage.Blobs;

namespace AK_Services.Services;

public class FileService(string conncetionString) : IFileService
{
    private string containerName = "save-files";
    private string _connectionString = conncetionString;
    public Task<string> SaveFileAsync(IFormFile file)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not set.");
        }
        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
        if (!containerClient.Exists())
        {
            containerClient.Create();
        }
        string fileName = $"{Guid.NewGuid()}_{file.FileName}";
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        using (var stream = file.OpenReadStream())
        {
            blobClient.Upload(stream, true);
        }
        return Task.FromResult(blobClient.Uri.AbsoluteUri.ToString());
    }

    public Task DeleteFileAsync(string fileName)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not set.");
        }
        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
        if (!containerClient.Exists())
        {
            throw new InvalidOperationException($"Container '{containerName}' does not exist.");
        }
        BlobClient blobClient = containerClient.GetBlobClient(fileName);
        if (blobClient.Exists())
        {
            blobClient.Delete();
            return Task.CompletedTask;
        }
        else
        {
            throw new FileNotFoundException($"File '{fileName}' not found in container '{containerName}'.");
        }
    }

    public Task EditFileAsync(IFormFile file, string path)
    {
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new InvalidOperationException("Azure Storage connection string is not set.");
        }
        BlobContainerClient containerClient = new BlobContainerClient(_connectionString, containerName);
        if (!containerClient.Exists())
        {
            throw new InvalidOperationException($"Container '{containerName}' does not exist.");
        }
        BlobClient blobClient = containerClient.GetBlobClient(path);
        using (var stream = file.OpenReadStream())
        {
            blobClient.Upload(stream, true);
        }
        return Task.CompletedTask;
    }
}