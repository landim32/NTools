using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Options;
using NTools.Domain.Services.Interfaces;
using NTools.DTO.Settings;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTools.Domain.Services
{
    public class FileService : IFileService
    {
        private readonly IOptions<S3Setting> _s3Settings;

        public FileService(IOptions<S3Setting> s3Settings)
        {
            _s3Settings = s3Settings;
        }

        public string GetFileUrl(string bucketName, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return _s3Settings.Value.Endpoint + "/" + bucketName + "/" + fileName;
            }
            return string.Empty;
        }

        public async Task<byte[]> DownloadFile(string url)
        {
            using (var httpClient = new HttpClient())
            {
                return await httpClient.GetByteArrayAsync(url);
            }
        }

        private void UploadFile(Stream fileStream, string bucketName, string fileName)
        {
            var config = new AmazonS3Config
            {
                ServiceURL = _s3Settings.Value.Endpoint,
                ForcePathStyle = true,
                //SignatureVersion = "v4",
            };

            using var client = new AmazonS3Client(_s3Settings.Value.AccessKey, _s3Settings.Value.SecretKey, config);
            var transferUtility = new TransferUtility(client);

            var request = new TransferUtilityUploadRequest
            {
                InputStream = fileStream,
                Key = fileName,
                BucketName = bucketName,
                CannedACL = S3CannedACL.PublicRead // ou Private se quiser restrito
            };

            transferUtility.Upload(request);
        }

        public string InsertFromStream(Stream stream, string bucketName, string name)
        {
            UploadFile(stream, bucketName, name);
            return name;
        }
    }
}
