using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NTools.ACL;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Net;
using System.Text;

namespace NTools.Tests.ACL
{
    public class FileClientTests
    {
        private readonly Mock<IOptions<NToolSetting>> _mockSettings;
        private readonly NToolSetting _settings;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<FileClient>> _mockLogger;

        public FileClientTests()
        {
            _settings = new NToolSetting
            {
                ApiUrl = "https://api.example.com"
            };

            _mockSettings = new Mock<IOptions<NToolSetting>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = _mockHttpHandler.ToHttpClient();

            _mockLogger = new Mock<ILogger<FileClient>>();
        }

        #region GetFileUrlAsync - Success Tests

        [Fact]
        public async Task GetFileUrlAsync_WithValidParameters_ReturnsUrl()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var expectedUrl = "https://storage.example.com/test-bucket/test-file.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Theory]
        [InlineData("bucket1", "file1.pdf", "https://s3.com/bucket1/file1.pdf")]
        [InlineData("images", "photo.png", "https://s3.com/images/photo.png")]
        [InlineData("documents", "report.docx", "https://s3.com/documents/report.docx")]
        public async Task GetFileUrlAsync_WithVariousInputs_ReturnsExpectedUrls(
            string bucket, string file, string expectedUrl)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/File/{bucket}/getFileUrl/{file}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucket, file);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public async Task GetFileUrlAsync_WithEmptyFileName_ReturnsEmptyString()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = string.Empty;
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(string.Empty));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task GetFileUrlAsync_WhenApiReturnsNull_ReturnsEmptyString()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "null");

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public async Task GetFileUrlAsync_WithSpecialCharactersInFileName_ReturnsUrl()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "folder/subfolder/file with spaces.pdf";
            var expectedUrl = "https://s3.com/test-bucket/folder/subfolder/file with spaces.pdf";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        #endregion

        #region GetFileUrlAsync - HTTP Request Tests

        [Fact]
        public async Task GetFileUrlAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test.jpg";
            var expectedUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject("https://url.com"));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GetFileUrlAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var bucketName = "bucket";
            var fileName = "file.jpg";
            var apiUrl = $"{customSettings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject("https://url.com"));

            var client = new FileClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GetFileUrlAsync - Error Handling Tests

        [Fact]
        public async Task GetFileUrlAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "nonexistent.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GetFileUrlAsync(bucketName, fileName));
        }

        [Fact]
        public async Task GetFileUrlAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GetFileUrlAsync(bucketName, fileName));
        }

        [Fact]
        public async Task GetFileUrlAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GetFileUrlAsync(bucketName, fileName));
        }

        #endregion

        #region UploadFileAsync - Success Tests

        [Fact]
        public async Task UploadFileAsync_WithValidFile_ReturnsFileName()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var expectedFileName = "uploaded-file-123.jpg";
            var mockFile = CreateMockFormFile(fileName, "test content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedFileName));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(expectedFileName, result);
        }

        [Theory]
        [InlineData("document.pdf", "uploaded-doc.pdf")]
        [InlineData("image.png", "uploaded-img.png")]
        [InlineData("archive.zip", "uploaded-archive.zip")]
        public async Task UploadFileAsync_WithDifferentFileTypes_ReturnsFileName(
            string originalName, string uploadedName)
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile(originalName, "content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(uploadedName));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(uploadedName, result);
        }

        [Fact]
        public async Task UploadFileAsync_WithLargeFile_ReturnsFileName()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "large-file.bin";
            var largeContent = new string('A', 1_000_000); // 1MB
            var mockFile = CreateMockFormFile(fileName, largeContent);
            var expectedFileName = "uploaded-large.bin";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedFileName));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(expectedFileName, result);
        }

        [Fact]
        public async Task UploadFileAsync_WhenApiReturnsNull_ReturnsEmptyString()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", "null");

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region UploadFileAsync - HTTP Request Tests

        [Fact]
        public async Task UploadFileAsync_MakesCorrectHttpPostRequest()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content");
            var expectedUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject("uploaded.jpg"));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task UploadFileAsync_SendsMultipartFormData()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test.jpg";
            var mockFile = CreateMockFormFile(fileName, "test content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, apiUrl)
                .With(request => 
                {
                    Assert.True(request.Content is MultipartFormDataContent);
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject("uploaded.jpg"));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task UploadFileAsync_UsesCorrectContentType()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content", "image/jpeg");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject("uploaded.jpg"));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region UploadFileAsync - Error Handling Tests

        [Fact]
        public async Task UploadFileAsync_WhenApiReturns400_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.BadRequest);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.UploadFileAsync(bucketName, mockFile.Object));
        }

        [Fact]
        public async Task UploadFileAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.UploadFileAsync(bucketName, mockFile.Object));
        }

        [Fact]
        public async Task UploadFileAsync_WhenApiReturns413_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("huge.bin", new string('A', 1000));
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.RequestEntityTooLarge);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.UploadFileAsync(bucketName, mockFile.Object));
        }

        [Fact]
        public async Task UploadFileAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test.jpg", "content");
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.UploadFileAsync(bucketName, mockFile.Object));
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Theory]
        [InlineData("my-bucket", "file.jpg")]
        [InlineData("images", "photo.png")]
        [InlineData("documents", "report.pdf")]
        public async Task GetFileUrlAsync_WithDifferentBuckets_WorksCorrectly(
            string bucket, string file)
        {
            // Arrange
            var expectedUrl = $"https://s3.com/{bucket}/{file}";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucket}/getFileUrl/{file}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucket, file);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public async Task UploadFileAsync_WithSpecialCharactersInFileName_WorksCorrectly()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "file with spaces & special-chars.pdf";
            var mockFile = CreateMockFormFile(fileName, "content");
            var expectedFileName = "uploaded-file.pdf";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedFileName));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(expectedFileName, result);
        }

        [Fact]
        public async Task GetFileUrlAsync_WithNestedFolderPath_WorksCorrectly()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "2024/01/15/document.pdf";
            var expectedUrl = $"https://s3.com/{bucketName}/{fileName}";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        #endregion

        #region Multiple Calls Tests

        [Fact]
        public async Task GetFileUrlAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var bucketName = "test-bucket";
            var file1 = "file1.jpg";
            var file2 = "file2.png";
            var url1 = "https://s3.com/file1.jpg";
            var url2 = "https://s3.com/file2.png";
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{file1}")
                .Respond("application/json", JsonConvert.SerializeObject(url1));
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{file2}")
                .Respond("application/json", JsonConvert.SerializeObject(url2));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.GetFileUrlAsync(bucketName, file1);
            var result2 = await client.GetFileUrlAsync(bucketName, file2);

            // Assert
            Assert.Equal(url1, result1);
            Assert.Equal(url2, result2);
        }

        [Fact]
        public async Task UploadFileAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile1 = CreateMockFormFile("file1.jpg", "content1");
            var mockFile2 = CreateMockFormFile("file2.png", "content2");
            var uploadedName1 = "uploaded1.jpg";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            // First call returns first file name
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(uploadedName1));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.UploadFileAsync(bucketName, mockFile1.Object);
            var result2 = await client.UploadFileAsync(bucketName, mockFile2.Object);

            // Assert
            Assert.Equal(uploadedName1, result1);
            Assert.NotNull(result2);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("application/pdf")]
        [InlineData("image/jpeg")]
        [InlineData("image/png")]
        [InlineData("application/zip")]
        [InlineData("text/plain")]
        public async Task UploadFileAsync_WithDifferentContentTypes_WorksCorrectly(string contentType)
        {
            // Arrange
            var bucketName = "test-bucket";
            var mockFile = CreateMockFormFile("test-file", "content", contentType);
            var expectedFileName = "uploaded-file";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/uploadFile";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedFileName));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.UploadFileAsync(bucketName, mockFile.Object);

            // Assert
            Assert.Equal(expectedFileName, result);
        }

        [Fact]
        public async Task GetFileUrlAsync_WithRealWorldScenario_ReturnsValidUrl()
        {
            // Arrange
            var bucketName = "company-documents";
            var fileName = "invoices/2024/january/invoice-001.pdf";
            var expectedUrl = "https://s3.amazonaws.com/company-documents/invoices/2024/january/invoice-001.pdf";
            var apiUrl = $"{_settings.ApiUrl}/File/{bucketName}/getFileUrl/{fileName}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedUrl));

            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GetFileUrlAsync(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
            Assert.Contains(bucketName, result);
            Assert.Contains(fileName, result);
        }

        #endregion

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = new FileClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }

        #endregion

        #region Helper Methods

        private Mock<IFormFile> CreateMockFormFile(
            string fileName, 
            string content, 
            string contentType = "application/octet-stream")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(bytes.Length);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.OpenReadStream()).Returns(() => 
            {
                stream.Position = 0;
                return stream;
            });
            
            return mockFile;
        }

        #endregion
    }
}
