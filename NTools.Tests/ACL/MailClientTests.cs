using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NTools.ACL;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Net;

namespace NTools.Tests.ACL
{
    public class MailClientTests
    {
        private readonly Mock<IOptions<NToolSetting>> _mockSettings;
        private readonly NToolSetting _settings;
        private readonly MockHttpMessageHandler _mockHttpHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<MailClient>> _mockLogger;

        public MailClientTests()
        {
            _settings = new NToolSetting
            {
                ApiUrl = "https://api.example.com"
            };

            _mockSettings = new Mock<IOptions<NToolSetting>>();
            _mockSettings.Setup(x => x.Value).Returns(_settings);

            _mockHttpHandler = new MockHttpMessageHandler();
            _httpClient = _mockHttpHandler.ToHttpClient();

            _mockLogger = new Mock<ILogger<MailClient>>();
        }

        #region IsValidEmailAsync - Success Tests

        [Fact]
        public async Task IsValidEmailAsync_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidEmailAsync_WithInvalidEmail_ReturnsFalse()
        {
            // Arrange
            var email = "invalid-email";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("test@example.com", true)]
        [InlineData("user.name@example.com", true)]
        [InlineData("user+tag@example.co.uk", true)]
        [InlineData("invalid", false)]
        [InlineData("@example.com", false)]
        [InlineData("user@", false)]
        public async Task IsValidEmailAsync_WithVariousEmails_ReturnsExpectedResult(
            string email, bool expectedResult)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(expectedResult));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user_name@example.org")]
        [InlineData("123@example.com")]
        [InlineData("test@subdomain.example.com")]
        public async Task IsValidEmailAsync_WithDifferentValidEmails_ReturnsTrue(string email)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid")]
        [InlineData("invalid@")]
        [InlineData("@example.com")]
        public async Task IsValidEmailAsync_WithInvalidEmails_ReturnsFalse(string email)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region IsValidEmailAsync - HTTP Request Tests

        [Fact]
        public async Task IsValidEmailAsync_MakesCorrectHttpGetRequest()
        {
            // Arrange
            var email = "test@example.com";
            var expectedUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .Expect(HttpMethod.Get, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.IsValidEmailAsync(email);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task IsValidEmailAsync_UsesCorrectApiUrl()
        {
            // Arrange
            var customSettings = new NToolSetting { ApiUrl = "https://custom-api.com" };
            var mockCustomSettings = new Mock<IOptions<NToolSetting>>();
            mockCustomSettings.Setup(x => x.Value).Returns(customSettings);

            var email = "test@example.com";
            var apiUrl = $"{customSettings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, mockCustomSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsValidEmailAsync - Error Handling Tests

        [Fact]
        public async Task IsValidEmailAsync_WhenApiReturns404_ThrowsHttpRequestException()
        {
            // Arrange
            var email = "test@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.NotFound);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.IsValidEmailAsync(email));
        }

        [Fact]
        public async Task IsValidEmailAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var email = "test@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.IsValidEmailAsync(email));
        }

        [Fact]
        public async Task IsValidEmailAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var email = "test@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.IsValidEmailAsync(email));
        }

        #endregion

        #region SendmailAsync - Success Tests

        [Fact]
        public async Task SendmailAsync_WithValidEmail_ReturnsTrue()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithValidEmail_ReturnsFalse()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendmailAsync_WithMultipleRecipients_ReturnsTrue()
        {
            // Arrange
            var mailInfo = new MailerInfo
            {
                From = new MailerRecipientInfo { Email = "sender@example.com", Name = "Sender" },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo { Email = "recipient1@example.com", Name = "Recipient 1" },
                    new MailerRecipientInfo { Email = "recipient2@example.com", Name = "Recipient 2" },
                    new MailerRecipientInfo { Email = "recipient3@example.com", Name = "Recipient 3" }
                },
                Subject = "Test Email",
                Text = "Test content",
                Html = "<p>Test content</p>"
            };
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithHtmlContent_ReturnsTrue()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Html = "<html><body><h1>Test Email</h1><p>This is a test</p></body></html>";
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithTextContent_ReturnsTrue()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = "This is plain text email content.";
            mailInfo.Html = null;
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region SendmailAsync - HTTP Request Tests

        [Fact]
        public async Task SendmailAsync_MakesCorrectHttpPostRequest()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var expectedUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, expectedUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendmailAsync(mailInfo);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task SendmailAsync_SendsJsonContent()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, apiUrl)
                .With(request => 
                {
                    Assert.NotNull(request.Content);
                    Assert.Equal("application/json", request.Content.Headers.ContentType?.MediaType);
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendmailAsync(mailInfo);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task SendmailAsync_SendsUtf8Encoding()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, apiUrl)
                .With(request => 
                {
                    Assert.NotNull(request.Content);
                    Assert.Contains("utf-8", request.Content.Headers.ContentType?.ToString() ?? "");
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendmailAsync(mailInfo);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
        }

        [Fact]
        public async Task SendmailAsync_SerializesMailInfoCorrectly()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            var serializedContent = string.Empty;
            
            _mockHttpHandler
                .Expect(HttpMethod.Post, apiUrl)
                .With(request => 
                {
                    serializedContent = request.Content!.ReadAsStringAsync().GetAwaiter().GetResult();
                    Assert.Contains(mailInfo.Subject, serializedContent);
                    return true;
                })
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            await client.SendmailAsync(mailInfo);

            // Assert
            _mockHttpHandler.VerifyNoOutstandingExpectation();
            Assert.NotEmpty(serializedContent);
        }

        #endregion

        #region SendmailAsync - Error Handling Tests

        [Fact]
        public async Task SendmailAsync_WhenApiReturns400_ThrowsHttpRequestException()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.BadRequest);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.SendmailAsync(mailInfo));
        }

        [Fact]
        public async Task SendmailAsync_WhenApiReturns500_ThrowsHttpRequestException()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.InternalServerError);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.SendmailAsync(mailInfo));
        }

        [Fact]
        public async Task SendmailAsync_WhenApiReturns401_ThrowsHttpRequestException()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.Unauthorized);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.SendmailAsync(mailInfo));
        }

        [Fact]
        public async Task SendmailAsync_WhenApiReturns429_ThrowsHttpRequestException()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond(HttpStatusCode.TooManyRequests);

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                client.SendmailAsync(mailInfo));
        }

        #endregion

        #region Edge Cases and Special Scenarios

        [Fact]
        public async Task IsValidEmailAsync_WithSpecialCharacters_WorksCorrectly()
        {
            // Arrange
            var email = "user+tag@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithUnicodeSubject_WorksCorrectly()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = "???? - Test Email - ???????? ??????";
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithUnicodeContent_WorksCorrectly()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = "Unicode content: ???? ?????? ??? ????? ???????";
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithLongContent_WorksCorrectly()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Text = new string('A', 10000);
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithEmptySubject_WorksCorrectly()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = string.Empty;
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Multiple Calls Tests

        [Fact]
        public async Task IsValidEmailAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var email1 = "test1@example.com";
            var email2 = "test2@example.com";
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/Mail/isValidEmail/{email1}")
                .Respond("application/json", JsonConvert.SerializeObject(true));
            
            _mockHttpHandler
                .When($"{_settings.ApiUrl}/Mail/isValidEmail/{email2}")
                .Respond("application/json", JsonConvert.SerializeObject(false));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.IsValidEmailAsync(email1);
            var result2 = await client.IsValidEmailAsync(email2);

            // Assert
            Assert.True(result1);
            Assert.False(result2);
        }

        [Fact]
        public async Task SendmailAsync_CalledMultipleTimes_EachCallSucceeds()
        {
            // Arrange
            var mailInfo1 = CreateValidMailInfo();
            mailInfo1.Subject = "First Email";
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result1 = await client.SendmailAsync(mailInfo1);
            var result2 = await client.SendmailAsync(mailInfo1);

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("Welcome to our service")]
        [InlineData("Password Reset Request")]
        [InlineData("Your order has been shipped")]
        [InlineData("Meeting Reminder")]
        public async Task SendmailAsync_WithDifferentSubjects_ReturnsTrue(string subject)
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            mailInfo.Subject = subject;
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithRealWorldScenario_WorksCorrectly()
        {
            // Arrange
            var mailInfo = new MailerInfo
            {
                From = new MailerRecipientInfo 
                { 
                    Email = "noreply@company.com", 
                    Name = "Company Name" 
                },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo 
                    { 
                        Email = "customer@example.com", 
                        Name = "John Doe" 
                    }
                },
                Subject = "Your Order Confirmation #12345",
                Text = "Thank you for your order. Your order #12345 has been confirmed.",
                Html = "<html><body><h1>Order Confirmation</h1><p>Thank you for your order.</p></body></html>"
            };
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("user_name@example.org")]
        public async Task IsValidEmailAsync_WithCommonEmailFormats_ReturnsTrue(string email)
        {
            // Arrange
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", JsonConvert.SerializeObject(true));

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region JSON Deserialization Tests

        [Fact]
        public async Task IsValidEmailAsync_WithJsonTrue_ReturnsTrue()
        {
            // Arrange
            var email = "test@example.com";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "true");

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsValidEmailAsync_WithJsonFalse_ReturnsFalse()
        {
            // Arrange
            var email = "invalid";
            var apiUrl = $"{_settings.ApiUrl}/Mail/isValidEmail/{email}";
            
            _mockHttpHandler
                .When(apiUrl)
                .Respond("application/json", "false");

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.IsValidEmailAsync(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendmailAsync_WithJsonTrue_ReturnsTrue()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", "true");

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task SendmailAsync_WithJsonFalse_ReturnsFalse()
        {
            // Arrange
            var mailInfo = CreateValidMailInfo();
            var apiUrl = $"{_settings.ApiUrl}/Mail/sendmail";
            
            _mockHttpHandler
                .When(HttpMethod.Post, apiUrl)
                .Respond("application/json", "false");

            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Act
            var result = await client.SendmailAsync(mailInfo);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Constructor and Initialization Tests

        [Fact]
        public void Constructor_WithValidParameters_CreatesInstance()
        {
            // Act
            var client = new MailClient(_httpClient, _mockSettings.Object, _mockLogger.Object);

            // Assert
            Assert.NotNull(client);
        }

        #endregion

        #region Helper Methods

        private MailerInfo CreateValidMailInfo()
        {
            return new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "sender@example.com",
                    Name = "Sender Name"
                },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo
                    {
                        Email = "recipient@example.com",
                        Name = "Recipient Name"
                    }
                },
                Subject = "Test Subject",
                Text = "Test content",
                Html = "<p>Test content</p>"
            };
        }

        #endregion
    }
}
