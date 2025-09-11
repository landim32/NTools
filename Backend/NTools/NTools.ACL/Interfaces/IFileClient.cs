using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTools.ACL.Interfaces
{
    public interface IFileClient
    {
        Task<string> GetFileUrlAsync(string bucketName, string fileName);
        Task<string> UploadFileAsync(string bucketName, IFormFile file);
    }
}
