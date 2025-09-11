using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.Domain.Interfaces.Services
{
    public interface IFileService
    {
        string GetFileUrl(string bucketName, string fileName);
        Task<byte[]> DownloadFile(string url);
        string InsertFromStream(Stream stream, string bucketName, string name);
    }
}
