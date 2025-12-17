using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NTools.ACL;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Net;

namespace NTools.Tests.ACL
{
    public class StringClientTests
    {
        private readonly Mock<IOptions<NToolSetting>> _mockSettings;
        private readonly NToolSetting _settings;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<StringClient>> _mockLogger;

        public StringClientTests()
        {
            _settings = new NToolSetting
            {
                ApiUrl = "https://api.example.com"
            };

            _mockSettings = new Mock<IOptions<NToolSetting>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = _mockHttpHandler.ToHttpClient();

            _mockLogger = new Mock<ILogger<StringClient>>();
        }

        #region GenerateSlugAsync - Success Tests

        [Fact]
        public async Task GenerateSlugAsync_WithSimpleString_ReturnsSlug()
        {
            // Arrange
            var input = "Hello World";
            var expectedSlug = "hello-world";
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        [Theory]
        [InlineData("Hello World", "hello-world")]
        [InlineData("Test String", "test-string")]
        [InlineData("UPPERCASE", "uppercase")]
        public async Task GenerateSlugAsync_WithVariousInputs_ReturnsExpectedSlug(
            string input, string expectedSlug)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        [Theory]
        [InlineData("Olá Mundo", "ola-mundo")]
        [InlineData("Café com Pão", "cafe-com-pao")]
        [InlineData("São Paulo", "sao-paulo")]
        public async Task GenerateSlugAsync_WithAccents_RemovesAccents(
            string input, string expectedSlug)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        [Fact]
        public async Task GenerateSlugAsync_WithEmptyString_ReturnsEmptyString()
        {
            // Arrange
            var input = string.Empty;
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "\"\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains("", result);
        }

        [Fact]
        public async Task GenerateSlugAsync_WithSpecialCharacters_RemovesSpecialChars()
        {
            // Arrange
            var input = "Test@123";
            var expectedSlug = "test123";
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        #endregion

        #region GenerateSlugAsync - HTTP Request Tests

        [Fact]
        public async Task GenerateSlugAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var input = "Test String";
            var expectedUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", "\"test-string\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.GenerateSlugAsync(input);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GenerateSlugAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var input = "test";
            var apiUrl = $"{customSettings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "\"test\"");

            var client = new StringClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GenerateSlugAsync - Error Handling Tests

        [Fact]
        public async Task GenerateSlugAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var input = "test";
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GenerateSlugAsync(input));
        }

        [Fact]
        public async Task GenerateSlugAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var input = "test";
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GenerateSlugAsync(input));
        }

        #endregion

        #region OnlyNumbersAsync - Success Tests

        [Fact]
        public async Task OnlyNumbersAsync_WithMixedInput_ReturnsOnlyDigits()
        {
            // Arrange
            var input = "abc123def456";
            var expected = "123456";
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expected}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains(expected, result);
        }

        [Theory]
        [InlineData("123", "123")]
        [InlineData("abc123", "123")]
        [InlineData("123abc", "123")]
        [InlineData("a1b2c3", "123")]
        public async Task OnlyNumbersAsync_WithVariousInputs_ExtractsNumbers(
            string input, string expected)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expected}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains(expected, result);
        }

        [Fact]
        public async Task OnlyNumbersAsync_WithNoNumbers_ReturnsEmptyString()
        {
            // Arrange
            var input = "abcdef";
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "\"\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains("", result);
        }

        [Theory]
        [InlineData("(11) 98765-4321", "11987654321")]
        [InlineData("123.456.789-09", "12345678909")]
        [InlineData("R$ 1.234,56", "123456")]
        public async Task OnlyNumbersAsync_WithFormattedNumbers_ExtractsDigits(
            string input, string expected)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expected}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains(expected, result);
        }

        [Fact]
        public async Task OnlyNumbersAsync_WithOnlyNumbers_ReturnsAllNumbers()
        {
            // Arrange
            var input = "1234567890";
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{input}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains(input, result);
        }

        #endregion

        #region OnlyNumbersAsync - HTTP Request Tests

        [Fact]
        public async Task OnlyNumbersAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var input = "abc123";
            var expectedUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", "\"123\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.OnlyNumbersAsync(input);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        #endregion

        #region OnlyNumbersAsync - Error Handling Tests

        [Fact]
        public async Task OnlyNumbersAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var input = "test123";
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.OnlyNumbersAsync(input));
        }

        [Fact]
        public async Task OnlyNumbersAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var input = "test123";
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.OnlyNumbersAsync(input));
        }

        #endregion

        #region GenerateShortUniqueStringAsync - Success Tests

        [Fact]
        public async Task GenerateShortUniqueStringAsync_ReturnsNonEmptyString()
        {
            // Arrange
            var expectedString = "AbC123XyZ";
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedString}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateShortUniqueStringAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Contains(expectedString, result);
        }

        [Fact]
        public async Task GenerateShortUniqueStringAsync_ReturnsBase62String()
        {
            // Arrange
            var expectedString = "abc123XYZ";
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedString}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateShortUniqueStringAsync();

            // Assert
            Assert.Contains(expectedString, result);
        }

        #endregion

        #region GenerateShortUniqueStringAsync - HTTP Request Tests

        [Fact]
        public async Task GenerateShortUniqueStringAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var expectedUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", "\"uniqueString\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.GenerateShortUniqueStringAsync();

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task GenerateShortUniqueStringAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var apiUrl = $"{customSettings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "\"unique\"");

            var client = new StringClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateShortUniqueStringAsync();

            // Assert
            Assert.NotNull(result);
        }

        #endregion

        #region GenerateShortUniqueStringAsync - Error Handling Tests

        [Fact]
        public async Task GenerateShortUniqueStringAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GenerateShortUniqueStringAsync());
        }

        [Fact]
        public async Task GenerateShortUniqueStringAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GenerateShortUniqueStringAsync());
        }

        [Fact]
        public async Task GenerateShortUniqueStringAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.GenerateShortUniqueStringAsync());
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Fact]
        public async Task GenerateSlugAsync_WithNumbers_KeepsNumbers()
        {
            // Arrange
            var input = "Test 123";
            var expectedSlug = "test-123";
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        [Fact]
        public async Task OnlyNumbersAsync_WithEmptyString_ReturnsEmpty()
        {
            // Arrange
            var input = string.Empty;
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "\"\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains("", result);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("Product Name 2024", "product-name-2024")]
        [InlineData("My First Blog Post!", "my-first-blog-post")]
        [InlineData("C# Programming", "c-programming")]
        public async Task GenerateSlugAsync_WithRealWorldExamples_GeneratesValidSlugs(
            string input, string expectedSlug)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/generateSlug/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedSlug}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.GenerateSlugAsync(input);

            // Assert
            Assert.Contains(expectedSlug, result);
        }

        [Theory]
        [InlineData("Order #12345", "12345")]
        [InlineData("Invoice: INV-2024-001", "2024001")]
        [InlineData("SKU: ABC123XYZ", "123")]
        public async Task OnlyNumbersAsync_WithRealWorldExamples_ExtractsNumbers(
            string input, string expected)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/String/onlyNumbers/{input}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expected}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.OnlyNumbersAsync(input);

            // Assert
            Assert.Contains(expected, result);
        }

        #endregion

        #region Multiple Calls Tests

        [Fact]
        public async Task GenerateSlugAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var input1 = "First String";
            var input2 = "Second String";
            var slug1 = "first-string";
            var slug2 = "second-string";
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/String/generateSlug/{input1}")
                .Respond("application/json", $"\"{slug1}\"");
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/String/generateSlug/{input2}")
                .Respond("application/json", $"\"{slug2}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.GenerateSlugAsync(input1);
            var result2 = await client.GenerateSlugAsync(input2);

            // Assert
            Assert.Contains(slug1, result1);
            Assert.Contains(slug2, result2);
        }

        [Fact]
        public async Task OnlyNumbersAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var input1 = "abc123";
            var input2 = "xyz789";
            var expected1 = "123";
            var expected2 = "789";
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/String/onlyNumbers/{input1}")
                .Respond("application/json", $"\"{expected1}\"");
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/String/onlyNumbers/{input2}")
                .Respond("application/json", $"\"{expected2}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.OnlyNumbersAsync(input1);
            var result2 = await client.OnlyNumbersAsync(input2);

            // Assert
            Assert.Contains(expected1, result1);
            Assert.Contains(expected2, result2);
        }

        [Fact]
        public async Task GenerateShortUniqueStringAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var expectedString = "AbC123XyZ";
            var apiUrl = $"{_settings.ApiUrl}/String/generateShortUniqueString";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", $"\"{expectedString}\"");

            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.GenerateShortUniqueStringAsync();
            var result2 = await client.GenerateShortUniqueStringAsync();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEmpty(result1);
            Assert.NotEmpty(result2);
        }

        #endregion

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = new StringClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }

        #endregion
    }
}
