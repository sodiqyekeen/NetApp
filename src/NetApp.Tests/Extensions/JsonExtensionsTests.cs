using global::NetApp.Extensions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace NetApp.Tests.Extensions;




public class JsonExtensionsTest
{
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

    [Fact]
    public void FromJson_NullArgument_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => json!.FromJson<object>());
    }

    [Fact]
    public void FromJson_ValidJson_ReturnsDeserializedObject()
    {
        // Arrange
        string json = "{\"id\": 1, \"name\": \"test\"}";

        // Act
        var result = json.FromJson<TestModel>();

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public void ToJson_ValidObject_ReturnsSerializedJson()
    {
        // Arrange
        var obj = new TestModel { Id = 1, Name = "test" };

        // Act
        var result = obj.ToJson();

        // Assert
        Assert.Equal("{\"Id\":1,\"Name\":\"test\"}", result);
    }

    [Fact]
    public void FromBytes_ValidBytes_ReturnsDeserializedObject()
    {
        // Arrange
        var obj = new TestModel { Id = 1, Name = "test" };
        var json = JsonSerializer.Serialize(obj, _jsonOptions);
        var bytes = Encoding.UTF8.GetBytes(json);

        // Act
        var result = bytes.FromBytes<TestModel>();

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("test", result.Name);
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        string json = "{\"id\": 1, \"name\": \"test\"";

        // Act & Assert
        Assert.Throws<JsonException>(() => json.FromJson<TestModel>());
    }

    

    [Fact]
    public void FromJson_MocksHttpClient_ReturnsDeserializedObject()
    {
        // Arrange
        string json = "{\"id\": 1, \"name\": \"test\"}";
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            });

        var client = new HttpClient(handlerMock.Object) { BaseAddress = new Uri("http://localhost") };
        var response = client.GetAsync("/test").Result;
        var result = response.Content.ReadAsStringAsync().Result.FromJson<TestModel>();

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("test", result.Name);
    }
}

public class TestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class InvalidTestModel { }

