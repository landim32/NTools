using Microsoft.AspNetCore.Http;

namespace NTools.ACL.Interfaces
{
    public interface IFileClient
    {
        Task<string> GetFileUrlAsync(string bucketName, string fileName);
        Task<string> UploadFileAsync(string bucketName, IFormFile file);
    }
}
