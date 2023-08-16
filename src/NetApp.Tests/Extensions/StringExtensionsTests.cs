using System.Text;

namespace NetApp.Tests.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void TestRandomTokenStringReturnsUniqueValues()
    {
        // Act 
        var firstRandomTokenString = NetApp.Extensions.StringExtensions.RandomTokenString();
        var secondRandomTokenString = NetApp.Extensions.StringExtensions.RandomTokenString();

        // Assert
        Assert.NotEqual(firstRandomTokenString, secondRandomTokenString);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void TestGenerateRandomNumberReturnsValuesWithCorrectPatter(int length)
    {
        // Act
        string result = NetApp.Extensions.StringExtensions.GenerateRandomNumber(length);
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Length == length);
        Assert.Matches("[0-9]+", result);
    }

    [Fact]
    public void TestParseBase64StringWithoutPaddingReturnsExpectedResult()
    {
        // Arrange
        string expectedString = "Test string to be encoded with base64";
        byte[] expectedBytes = Encoding.UTF8.GetBytes(expectedString);
        string base64 = Convert.ToBase64String(expectedBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
        // Act
        byte[] result = NetApp.Extensions.StringExtensions.ParseBase64StringWithoutPadding(base64);
        string resultString = Encoding.UTF8.GetString(result);
        // Assert
        Assert.Equal(expectedString, resultString);
    }
}


