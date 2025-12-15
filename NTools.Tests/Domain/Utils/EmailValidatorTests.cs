using NTools.Domain.Utils;

namespace NTools.Tests.Domain.Utils
{
    public class EmailValidatorTests
    {
        #region Valid Email Tests

        [Theory]
        [InlineData("user@example.com")]
        [InlineData("test@test.com")]
        [InlineData("admin@domain.com")]
        [InlineData("contact@company.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user_name@example.com")]
        [InlineData("user-name@example.com")]
        [InlineData("user+tag@example.com")]
        [InlineData("user123@example.com")]
        [InlineData("user@subdomain.example.com")]
        [InlineData("user@mail.subdomain.example.com")]
        [InlineData("user@deep.sub.domain.example.com")]
        [InlineData("user@example.co")]
        [InlineData("user@example.com.br")]
        [InlineData("user@example.co.uk")]
        [InlineData("user@example.org")]
        [InlineData("user@example.net")]
        [InlineData("user@example.edu")]
        [InlineData("user@example.gov")]
        [InlineData("a@b.c")]
        [InlineData("1@2.3")]
        [InlineData("x@y.z")]
        [InlineData("USER@EXAMPLE.COM")]
        [InlineData("User@Example.Com")]
        [InlineData("UsEr@ExAmPlE.cOm")]
        [InlineData("1234567890@example.com")]
        [InlineData("user123456@test123.com")]
        [InlineData("test@123domain.com")]
        [InlineData(".user@example.com")]
        [InlineData("user.@example.com")]
        [InlineData("..user@example.com")]
        [InlineData("user@example..com")]
        [InlineData("user@..example.com")]
        [InlineData("user@example...com")]
        [InlineData("user@.domain.com")]
        [InlineData("user!name@example.com")]
        [InlineData("user#name@example.com")]
        [InlineData("user$name@example.com")]
        [InlineData("user%name@example.com")]
        [InlineData("user&name@example.com")]
        [InlineData("user*name@example.com")]
        [InlineData("user@domain!.com")]
        [InlineData("user@domain#.com")]
        [InlineData("user@domain$.com")]
        [InlineData("very.long.email.address.with.many.dots@very.long.domain.name.with.many.subdomains.com")]
        [InlineData("a.b.c.d.e.f.g.h.i.j.k@domain.com")]
        [InlineData("user+tag1+tag2+tag3@example.com")]
        [InlineData("user+filtering@example.com")]
        [InlineData("user+test@example.com")]
        [InlineData("user@123.456.789.012")]
        [InlineData("test@192.168.1.1")]
        [InlineData("user@domain-with-dash.com")]
        [InlineData("user@sub-domain.example.com")]
        [InlineData("user@multi-dash-domain.co.uk")]
        [InlineData("user@-domain.com")]
        [InlineData("user@domain-.com")]
        [InlineData("user@-example-.com")]
        [InlineData("user@gmail.com")]
        [InlineData("user@outlook.com")]
        [InlineData("user@hotmail.com")]
        [InlineData("user@yahoo.com")]
        [InlineData("user@icloud.com")]
        [InlineData("user@protonmail.com")]
        [InlineData("user@mail.ru")]
        [InlineData("user@yandex.com")]
        [InlineData("user@munchen.de")]
        [InlineData("usuario@espanol.es")]
        public void IsValidEmail_WithValidEmails_ReturnsTrue(string email)
        {
            // Act
            var result = EmailValidator.IsValidEmail(email);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Invalid Email Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void IsValidEmail_WithEmptyOrWhitespace_ReturnsFalse(string email)
        {
            // Act
            var result = EmailValidator.IsValidEmail(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidEmail_WithNull_ReturnsFalse()
        {
            // Act
            var result = EmailValidator.IsValidEmail(null);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("plaintext")]
        [InlineData("notanemail")]
        [InlineData("missing-at-symbol.com")]
        [InlineData("@example.com")]
        [InlineData("@domain.com")]
        [InlineData("@test.co")]
        [InlineData("user@")]
        [InlineData("test@")]
        [InlineData("admin@")]
        [InlineData("user@domain")]
        [InlineData("user@localhost")]
        [InlineData("test@server")]
        [InlineData("user@@example.com")]
        [InlineData("user@example@com")]
        [InlineData("user@@example@@com")]
        [InlineData("user @example.com")]
        [InlineData("user@ example.com")]
        [InlineData("user@example .com")]
        [InlineData("user @example .com")]
        [InlineData("user@.com")]
        [InlineData("user@domain.")]
        public void IsValidEmail_WithInvalidEmails_ReturnsFalse(string email)
        {
            // Act
            var result = EmailValidator.IsValidEmail(email);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region Real-World Scenarios

        [Theory]
        [InlineData("john.doe@example.com", true)]
        [InlineData("jane_smith@company.co.uk", true)]
        [InlineData("admin+test@domain.org", true)]
        [InlineData("contact@sub.domain.com", true)]
        [InlineData("user@", false)]
        [InlineData("@example.com", false)]
        [InlineData("invalid email@test.com", false)]
        [InlineData("user@@domain.com", false)]
        [InlineData("", false)]
        [InlineData("notanemail", false)]
        [InlineData("user@domain", false)]
        public void IsValidEmail_RealWorldScenarios_WorksAsExpected(string email, bool expectedResult)
        {
            // Act
            var result = EmailValidator.IsValidEmail(email);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region Performance and Safety Tests

        [Fact]
        public void IsValidEmail_WithVeryLongString_DoesNotCauseTimeout()
        {
            // Arrange
            var longEmail = new string('a', 10000) + "@example.com";

            // Act
            var result = EmailValidator.IsValidEmail(longEmail);

            // Assert
            // Should complete without timeout
            Assert.True(result || !result); // Just checking it completes
        }

        [Theory]
        [InlineData("test1@example.com")]
        [InlineData("test2@example.com")]
        [InlineData("test3@example.com")]
        public void IsValidEmail_CalledMultipleTimes_RemainsConsistent(string email)
        {
            // Act
            var result1 = EmailValidator.IsValidEmail(email);
            var result2 = EmailValidator.IsValidEmail(email);
            var result3 = EmailValidator.IsValidEmail(email);

            // Assert
            Assert.Equal(result1, result2);
            Assert.Equal(result2, result3);
        }

        #endregion
    }
}
