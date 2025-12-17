using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NTools.ACL;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Net;

namespace NTools.Tests.ACL
{
    public class DocumentClientTests
    {
        private readonly Mock<IOptions<NToolSetting>> _mockSettings;
        private readonly NToolSetting _settings;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<DocumentClient>> _mockLogger;

        public DocumentClientTests()
        {
            _settings = new NToolSetting
            {
                ApiUrl = "https://api.example.com"
            };

            _mockSettings = new Mock<IOptions<NToolSetting>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = _mockHttpHandler.ToHttpClient();

            _mockLogger = new Mock<ILogger<DocumentClient>>();
        }

        #region ValidarCpfOuCnpjAsync - Success Tests

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithValidCpf_ReturnsTrue()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithInvalidCpf_ReturnsFalse()
        {
            // Arrange
            var cpf = "00000000000";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("12345678909", true)]
        [InlineData("123.456.789-09", true)]
        [InlineData("11222333000181", true)]
        [InlineData("11.222.333/0001-81", true)]
        [InlineData("00000000000", false)]
        [InlineData("invalid", false)]
        public async Task ValidarCpfOuCnpjAsync_WithVariousInputs_ReturnsExpectedResult(string document, bool expectedResult)
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{document}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResult));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(document);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithValidCnpj_ReturnsTrue()
        {
            // Arrange
            var cnpj = "11222333000181";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cnpj}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cnpj);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithFormattedCpf_ReturnsTrue()
        {
            // Arrange
            var cpf = "123.456.789-09";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithFormattedCnpj_ReturnsTrue()
        {
            // Arrange
            var cnpj = "11.222.333/0001-81";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cnpj}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cnpj);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - HTTP Request Tests

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var cpf = "12345678909";
            var expectedUrl = $"{customSettings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithSpecialCharactersInDocument_EncodesUrl()
        {
            // Arrange
            var document = "123.456.789-09";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{document}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(document);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - Error Handling Tests

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.validarCpfOuCnpjAsync(cpf));
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.validarCpfOuCnpjAsync(cpf));
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.validarCpfOuCnpjAsync(cpf));
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WhenApiReturns403_ThrowsHttpRequestException()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond(HttpStatusCode.Forbidden);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.validarCpfOuCnpjAsync(cpf));
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WhenApiReturnsBadRequest_ThrowsHttpRequestException()
        {
            // Arrange
            var cpf = "invalid";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond(HttpStatusCode.BadRequest);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.validarCpfOuCnpjAsync(cpf));
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - JSON Deserialization Tests

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithValidJsonTrue_ReturnsTrue()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", "true");

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithValidJsonFalse_ReturnsFalse()
        {
            // Arrange
            var cpf = "00000000000";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", "false");

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithJsonBooleanObject_DeserializesCorrectly()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            var jsonResponse = JsonConvert.SerializeObject(true);
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", jsonResponse);

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - Edge Cases

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithEmptyString_MakesRequest()
        {
            // Arrange
            var document = string.Empty;
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{document}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(document);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithWhitespace_MakesRequest()
        {
            // Arrange
            var document = "   ";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{document}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(document);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_WithVeryLongString_MakesRequest()
        {
            // Arrange
            var document = new string('1', 100);
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{document}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(document);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - Real-World Scenarios

        [Theory]
        [InlineData("12345678909")]
        [InlineData("123.456.789-09")]
        [InlineData("529.982.247-25")]
        [InlineData("111.444.777-35")]
        public async Task ValidarCpfOuCnpjAsync_WithValidCpfFormats_ReturnsTrue(string cpf)
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        [InlineData("34028316000103")]
        [InlineData("34.028.316/0001-03")]
        public async Task ValidarCpfOuCnpjAsync_WithValidCnpjFormats_ReturnsTrue(string cnpj)
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cnpj}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cnpj);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("99999999999")]
        public async Task ValidarCpfOuCnpjAsync_WithRepeatedDigitsCpf_ReturnsFalse(string cpf)
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("99999999999999")]
        public async Task ValidarCpfOuCnpjAsync_WithRepeatedDigitsCnpj_ReturnsFalse(string cnpj)
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cnpj}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.validarCpfOuCnpjAsync(cnpj);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region ValidarCpfOuCnpjAsync - Multiple Calls Tests

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_CalledMultipleTimes_EachCallMakesRequest()
        {
            // Arrange
            var cpf1 = "12345678909";
            var cpf2 = "98765432100";
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf1}")
                .Respond("application/json", JsonConvert.SerializeObject(true));
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf2}")
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.validarCpfOuCnpjAsync(cpf1);
            var result2 = await client.validarCpfOuCnpjAsync(cpf2);

            // Assert
            Assert.True(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task ValidarCpfOuCnpjAsync_SameDocumentCalledTwice_BothCallsSucceed()
        {
            // Arrange
            var cpf = "12345678909";
            var expectedUrl = $"{_settings.ApiUrl}/Document/validarCpfOuCnpj/{cpf}";
            
            _mockHttpHandler
                .When(expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.validarCpfOuCnpjAsync(cpf);
            var result2 = await client.validarCpfOuCnpjAsync(cpf);

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        #endregion

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = new DocumentClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }

        #endregion
    }
}
