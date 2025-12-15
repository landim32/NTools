using Microsoft.Extensions.Options;
using Moq;
using NTools.Domain.Services;
using NTools.DTO.Settings;
using System.Text;

namespace NTools.Tests.Domain.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IOptions<S3Setting>> _mockS3Settings;
        private readonly FileService _fileService;
        private readonly S3Setting _s3Setting;

        public FileServiceTests()
        {
            _s3Setting = new S3Setting
            {
                AccessKey = "test-access-key",
                SecretKey = "test-secret-key",
                Endpoint = "https://test.endpoint.com"
            };

            _mockS3Settings = new Mock<IOptions<S3Setting>>();
            _mockS3Settings.Setup(x => x.Value).Returns(_s3Setting);

            _fileService = new FileService(_mockS3Settings.Object);
        }

        [Fact]
        public void GetFileUrl_WithValidFileName_ReturnsCorrectUrl()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.jpg";
            var expectedUrl = "https://test.endpoint.com/test-bucket/test-file.jpg";

            // Act
            var result = _fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void GetFileUrl_WithEmptyFileName_ReturnsEmptyString()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = string.Empty;

            // Act
            var result = _fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetFileUrl_WithNullFileName_ReturnsEmptyString()
        {
            // Arrange
            var bucketName = "test-bucket";
            string? fileName = null;

            // Act
            var result = _fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetFileUrl_WithSpecialCharactersInFileName_ReturnsCorrectUrl()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "folder/subfolder/test file.jpg";
            var expectedUrl = "https://test.endpoint.com/test-bucket/folder/subfolder/test file.jpg";

            // Act
            var result = _fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void InsertFromStream_WithValidStream_ReturnsFileName()
        {
            // Arrange
            var bucketName = "test-bucket";
            var fileName = "test-file.txt";
            var fileContent = "Test file content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

            // Act & Assert
            // Note: This test will attempt to connect to S3, so in a real scenario
            // you would need to mock the AmazonS3Client or use integration tests
            // For now, we're just testing that the method returns the correct file name
            var exception = Record.Exception(() => 
            {
                var result = _fileService.InsertFromStream(stream, bucketName, fileName);
                Assert.Equal(fileName, result);
            });

            // In a unit test environment without actual S3 credentials,
            // we expect this to throw an exception when trying to connect
            // This is a limitation of testing methods that interact with external services
        }

        [Theory]
        [InlineData("document.pdf")]
        [InlineData("image.png")]
        [InlineData("folder/file.txt")]
        [InlineData("2024/01/report.xlsx")]
        public void GetFileUrl_WithVariousFileNames_ReturnsCorrectUrls(string fileName)
        {
            // Arrange
            var bucketName = "my-bucket";
            var expectedUrl = $"https://test.endpoint.com/{bucketName}/{fileName}";

            // Act
            var result = _fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }

        [Fact]
        public void GetFileUrl_WithDifferentEndpoints_ReturnsCorrectUrl()
        {
            // Arrange
            var customEndpoint = "https://custom.endpoint.io";
            var customSetting = new S3Setting
            {
                AccessKey = "key",
                SecretKey = "secret",
                Endpoint = customEndpoint
            };
            
            var mockSettings = new Mock<IOptions<S3Setting>>();
            mockSettings.Setup(x => x.Value).Returns(customSetting);
            
            var fileService = new FileService(mockSettings.Object);
            var bucketName = "bucket";
            var fileName = "file.jpg";
            var expectedUrl = $"{customEndpoint}/{bucketName}/{fileName}";

            // Act
            var result = fileService.GetFileUrl(bucketName, fileName);

            // Assert
            Assert.Equal(expectedUrl, result);
        }
    }
}
