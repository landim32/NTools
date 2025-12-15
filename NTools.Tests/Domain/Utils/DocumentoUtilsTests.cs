using NTools.Domain.Utils;

namespace NTools.Tests.Domain.Utils
{
    public class DocumentoUtilsTests
    {
        #region ValidarCpfOuCnpj Tests

        [Theory]
        [InlineData("12345678909")]
        [InlineData("123.456.789-09")]
        public void ValidarCpfOuCnpj_WithValidCpf_ReturnsTrue(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        public void ValidarCpfOuCnpj_WithValidCnpj_ReturnsTrue(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("12345678900")]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("123.456.789-00")]
        public void ValidarCpfOuCnpj_WithInvalidCpf_ReturnsFalse(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("11222333000180")]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("11.222.333/0001-80")]
        public void ValidarCpfOuCnpj_WithInvalidCnpj_ReturnsFalse(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("12345")]
        [InlineData("123456789")]
        [InlineData("12345678901234")]
        public void ValidarCpfOuCnpj_WithInvalidLength_ReturnsFalse(string documento)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(documento);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidarCpfOuCnpj_WithNull_ReturnsFalse()
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(null);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("abc.def.ghi-jk")]
        [InlineData("abc12345678")]
        [InlineData("!!!########")]
        public void ValidarCpfOuCnpj_WithNonNumericCharacters_ReturnsFalse(string documento)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(documento);

            // Assert
            Assert.False(result);
        }

        #endregion

        #region CPF Specific Tests

        [Theory]
        [InlineData("52998224725")]
        [InlineData("529.982.247-25")]
        [InlineData("111.444.777-35")]
        [InlineData("11144477735")]
        public void ValidarCpfOuCnpj_WithMultipleValidCpfs_ReturnsTrue(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("00000000000")]
        [InlineData("11111111111")]
        [InlineData("22222222222")]
        [InlineData("33333333333")]
        [InlineData("44444444444")]
        [InlineData("55555555555")]
        [InlineData("66666666666")]
        [InlineData("77777777777")]
        [InlineData("88888888888")]
        [InlineData("99999999999")]
        public void ValidarCpfOuCnpj_WithRepeatedDigitsCpf_ReturnsFalse(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("52998224726")]
        [InlineData("52998224724")]
        [InlineData("52998224735")]
        public void ValidarCpfOuCnpj_WithInvalidVerificationDigits_ReturnsFalse(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("529-982-247-25")]
        [InlineData("529 982 247 25")]
        [InlineData("529.982.247.25")]
        public void ValidarCpfOuCnpj_WithDifferentFormatting_ValidatesCpfCorrectly(string cpf)
        {
            // Arrange
            var validCpf = "52998224725";
            var expectedResult = DocumentoUtils.ValidarCpfOuCnpj(validCpf);

            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region CNPJ Specific Tests

        [Theory]
        [InlineData("11222333000181")]
        [InlineData("11.222.333/0001-81")]
        [InlineData("34028316000103")]
        [InlineData("34.028.316/0001-03")]
        public void ValidarCpfOuCnpj_WithMultipleValidCnpjs_ReturnsTrue(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("00000000000000")]
        [InlineData("11111111111111")]
        [InlineData("22222222222222")]
        [InlineData("33333333333333")]
        [InlineData("44444444444444")]
        [InlineData("55555555555555")]
        [InlineData("66666666666666")]
        [InlineData("77777777777777")]
        [InlineData("88888888888888")]
        [InlineData("99999999999999")]
        public void ValidarCpfOuCnpj_WithRepeatedDigitsCnpj_ReturnsFalse(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("11222333000182")]
        [InlineData("11222333000180")]
        [InlineData("11222333000191")]
        public void ValidarCpfOuCnpj_WithInvalidCnpjVerificationDigits_ReturnsFalse(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("11-222-333-0001-81")]
        [InlineData("11 222 333 0001 81")]
        [InlineData("11.222.333.0001.81")]
        public void ValidarCpfOuCnpj_WithDifferentCnpjFormatting_ValidatesCorrectly(string cnpj)
        {
            // Arrange
            var validCnpj = "11222333000181";
            var expectedResult = DocumentoUtils.ValidarCpfOuCnpj(validCnpj);

            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion

        #region Edge Cases

        [Theory]
        [InlineData("   12345678909   ")]
        [InlineData("   11222333000181   ")]
        public void ValidarCpfOuCnpj_WithWhitespace_HandlesCorrectly(string documento)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(documento);

            // Assert - The current implementation strips all non-digit characters
            // so this will validate the document itself
            Assert.True(result);
        }

        [Theory]
        [InlineData("123456789")]
        [InlineData("1234567890123")]
        [InlineData("123456789012")]
        public void ValidarCpfOuCnpj_WithWrongLength_ReturnsFalse(string documento)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(documento);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("000.000.001-91")]
        [InlineData("00000000191")]
        public void ValidarCpfOuCnpj_WithLeadingZerosCpf_ValidatesCorrectly(string cpf)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cpf);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("00.000.000/0001-91")]
        [InlineData("00000000000191")]
        public void ValidarCpfOuCnpj_WithLeadingZerosCnpj_ValidatesCorrectly(string cnpj)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(cnpj);

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Real-World Examples

        [Theory]
        [InlineData("12345678909", true)]
        [InlineData("111.444.777-35", true)]
        [InlineData("11222333000181", true)]
        [InlineData("11.222.333/0001-81", true)]
        [InlineData("12345678900", false)]
        [InlineData("11111111111", false)]
        [InlineData("11222333000180", false)]
        [InlineData("00000000000000", false)]
        [InlineData("", false)]
        [InlineData("abc", false)]
        public void ValidarCpfOuCnpj_RealWorldScenarios_WorksAsExpected(string documento, bool expectedResult)
        {
            // Act
            var result = DocumentoUtils.ValidarCpfOuCnpj(documento);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        #endregion
    }
}
