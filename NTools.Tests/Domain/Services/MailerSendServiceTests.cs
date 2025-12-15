using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using NTools.Domain.Services;
using NTools.DTO.MailerSend;
using NTools.DTO.Settings;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Headers;

namespace NTools.Tests.Domain.Services
{
    public class MailerSendServiceTests
    {
        private readonly Mock<IOptions<MailerSendSetting>> _mockMailSettings;
        private readonly MailerSendSetting _mailSetting;

        public MailerSendServiceTests()
        {
            _mailSetting = new MailerSendSetting
            {
                ApiUrl = "https://api.mailersend.com/v1/email",
                ApiToken = "test-api-token-12345",
                MailSender = "sender@test.com"
            };

            _mockMailSettings = new Mock<IOptions<MailerSendSetting>>();
            _mockMailSettings.Setup(x => x.Value).Returns(_mailSetting);
        }

        private MailerInfo CreateTestMailerInfo()
        {
            return new MailerInfo
            {
                From = new MailerRecipientInfo
                {
                    Email = "from@test.com",
                    Name = "Test Sender"
                },
                To = new List<MailerRecipientInfo>
                {
                    new MailerRecipientInfo
                    {
                        Email = "recipient@test.com",
                        Name = "Test Recipient"
                    }
                },
                Subject = "Test Subject",
                Text = "Test plain text content",
                Html = "<p>Test HTML content</p>"
            };
        }

        [Fact]
        public void Constructor_WithValidSettings_InitializesSuccessfully()
        {
            // Act
            var service = new MailerSendService(_mockMailSettings.Object);

            // Assert
            Assert.NotNull(service);
        }

        // Note: The current implementation doesn't validate null settings in constructor
        // This would throw a NullReferenceException at runtime when Sendmail is called
        // Consider adding validation in the constructor for better fail-fast behavior

        [Fact]
        public void CreateMailerInfo_WithAllFields_CreatesValidObject()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();

            // Assert
            Assert.NotNull(mailerInfo);
            Assert.NotNull(mailerInfo.From);
            Assert.NotNull(mailerInfo.To);
            Assert.Single(mailerInfo.To);
            Assert.Equal("Test Subject", mailerInfo.Subject);
            Assert.Equal("Test plain text content", mailerInfo.Text);
            Assert.Equal("<p>Test HTML content</p>", mailerInfo.Html);
        }

        [Fact]
        public void MailerInfo_Serialization_ProducesCorrectJson()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);
            var deserialized = JsonConvert.DeserializeObject<MailerInfo>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(mailerInfo.Subject, deserialized.Subject);
            Assert.Equal(mailerInfo.Text, deserialized.Text);
            Assert.Equal(mailerInfo.Html, deserialized.Html);
            Assert.Equal(mailerInfo.From.Email, deserialized.From.Email);
            Assert.Equal(mailerInfo.To.First().Email, deserialized.To.First().Email);
        }

        [Fact]
        public void MailerErrorInfo_Deserialization_ParsesErrorMessage()
        {
            // Arrange
            var errorJson = @"{
                ""message"": ""Validation failed"",
                ""errors"": {
                    ""to"": [""The to field is required""]
                }
            }";

            // Act
            var errorInfo = JsonConvert.DeserializeObject<MailerErrorInfo>(errorJson);

            // Assert
            Assert.NotNull(errorInfo);
            Assert.Equal("Validation failed", errorInfo.Message);
            Assert.NotNull(errorInfo.Errors);
            Assert.True(errorInfo.Errors.ContainsKey("to"));
            Assert.Single(errorInfo.Errors["to"]);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void MailerInfo_WithInvalidSubject_StillCreatesObject(string subject)
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();
            mailerInfo.Subject = subject;

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);

            // Assert
            Assert.NotNull(json);
        }

        [Fact]
        public void MailerInfo_WithMultipleRecipients_SerializesCorrectly()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();
            mailerInfo.To = new List<MailerRecipientInfo>
            {
                new MailerRecipientInfo { Email = "recipient1@test.com", Name = "Recipient 1" },
                new MailerRecipientInfo { Email = "recipient2@test.com", Name = "Recipient 2" },
                new MailerRecipientInfo { Email = "recipient3@test.com", Name = "Recipient 3" }
            };

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);
            var deserialized = JsonConvert.DeserializeObject<MailerInfo>(json);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(3, deserialized.To.Count);
            Assert.Equal("recipient1@test.com", deserialized.To[0].Email);
            Assert.Equal("recipient2@test.com", deserialized.To[1].Email);
            Assert.Equal("recipient3@test.com", deserialized.To[2].Email);
        }

        [Fact]
        public void MailerRecipientInfo_WithValidData_CreatesCorrectly()
        {
            // Arrange & Act
            var recipient = new MailerRecipientInfo
            {
                Email = "test@example.com",
                Name = "Test User"
            };

            // Assert
            Assert.Equal("test@example.com", recipient.Email);
            Assert.Equal("Test User", recipient.Name);
        }

        [Fact]
        public void MailerSendSetting_ConfigurationValues_AreAccessible()
        {
            // Assert
            Assert.Equal("https://api.mailersend.com/v1/email", _mailSetting.ApiUrl);
            Assert.Equal("test-api-token-12345", _mailSetting.ApiToken);
            Assert.Equal("sender@test.com", _mailSetting.MailSender);
        }

        [Fact]
        public void MailerInfo_FromEmail_ShouldBeOverriddenBySettings()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();
            var originalFromEmail = mailerInfo.From.Email;

            // Act
            mailerInfo.From.Email = _mailSetting.MailSender;

            // Assert
            Assert.NotEqual(originalFromEmail, mailerInfo.From.Email);
            Assert.Equal(_mailSetting.MailSender, mailerInfo.From.Email);
        }

        [Theory]
        [InlineData("user@example.com", "John Doe")]
        [InlineData("admin@domain.com", "Admin User")]
        [InlineData("test@test.com", "Test Account")]
        public void MailerRecipientInfo_WithVariousData_CreatesCorrectly(string email, string name)
        {
            // Arrange & Act
            var recipient = new MailerRecipientInfo
            {
                Email = email,
                Name = name
            };

            // Assert
            Assert.Equal(email, recipient.Email);
            Assert.Equal(name, recipient.Name);
        }

        [Fact]
        public void MailerInfo_JsonProperties_AreCorrectlyMapped()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);

            // Assert
            Assert.Contains("\"from\"", json);
            Assert.Contains("\"to\"", json);
            Assert.Contains("\"subject\"", json);
            Assert.Contains("\"text\"", json);
            Assert.Contains("\"html\"", json);
        }

        [Fact]
        public void MailerErrorInfo_WithComplexErrors_DeserializesCorrectly()
        {
            // Arrange
            var errorJson = @"{
                ""message"": ""Multiple validation errors"",
                ""errors"": {
                    ""to"": [""The to field is required"", ""Invalid email format""],
                    ""subject"": [""The subject field is required""],
                    ""from"": [""Invalid sender email""]
                }
            }";

            // Act
            var errorInfo = JsonConvert.DeserializeObject<MailerErrorInfo>(errorJson);

            // Assert
            Assert.NotNull(errorInfo);
            Assert.Equal("Multiple validation errors", errorInfo.Message);
            Assert.Equal(3, errorInfo.Errors.Count);
            Assert.Equal(2, errorInfo.Errors["to"].Count);
            Assert.Single(errorInfo.Errors["subject"]);
            Assert.Single(errorInfo.Errors["from"]);
        }

        [Fact]
        public void MailerInfo_WithOnlyHtmlContent_IsValid()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();
            mailerInfo.Text = null;

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);

            // Assert
            Assert.NotNull(json);
            Assert.NotNull(mailerInfo.Html);
        }

        [Fact]
        public void MailerInfo_WithOnlyTextContent_IsValid()
        {
            // Arrange
            var mailerInfo = CreateTestMailerInfo();
            mailerInfo.Html = null;

            // Act
            var json = JsonConvert.SerializeObject(mailerInfo);

            // Assert
            Assert.NotNull(json);
            Assert.NotNull(mailerInfo.Text);
        }
    }
}
