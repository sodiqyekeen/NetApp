using System.Text.Json;
namespace NetApp.Shared.Extensions;
public static class JsonExtentions
{
    private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public static T FromJson<T>(this string json) =>
        JsonSerializer.Deserialize<T>(json, _jsonOptions)!;

    public static string ToJson<T>(this T obj) =>
        JsonSerializer.Serialize(obj, _jsonOptions);
    
    public static T FromBytes<T>(this byte[] bytes) =>
        JsonSerializer.Deserialize<T>(bytes)!;
}