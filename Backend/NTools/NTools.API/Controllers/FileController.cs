using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NTools.Domain.Interfaces.Services;
using NTools.DTO.Domain;
using System;

namespace BazzucaMedia.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet("{bucketName}/getFileUrl/{fileName}")]
        public ActionResult<StringResult> GetFileUrl(string bucketName, string fileName)
        {
            try
            {
                return new StringResult()
                {
                    Value = _fileService.GetFileUrl(bucketName, fileName)
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [RequestSizeLimit(100_000_000)]
        [HttpPost("{bucketName}/uploadFile")]
        public ActionResult<StringResult> UploadFile(string bucketName, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("No file uploaded");
                }
                var fileName = _fileService.InsertFromStream(file.OpenReadStream(), bucketName, file.FileName);
                return new StringResult()
                {
                    Value = fileName
                };
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
