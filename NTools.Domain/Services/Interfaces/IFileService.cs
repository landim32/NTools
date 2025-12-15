using System.IO;
using System.Threading.Tasks;

namespace NTools.Domain.Services.Interfaces
{
    public interface IFileService
    {
        string GetFileUrl(string bucketName, string fileName);
        Task<byte[]> DownloadFile(string url);
        string InsertFromStream(Stream stream, string bucketName, string name);
    }
}
