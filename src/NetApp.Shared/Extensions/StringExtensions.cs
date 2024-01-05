using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace NetApp.Extensions;
public static class StringExtensions
{

    public static byte[] ParseBase64StringWithoutPadding(this string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;
            case 3:
                base64 += "=";
                break;
        }
        return Convert.FromBase64String(base64);
    }

    /// <summary>
    /// Returns the current page from the provided URI.
    /// </summary>
    /// <param name="uri">The URI to find the current page in.</param>
    /// <returns>A string representing the current page.</returns>
    public static string GetCurrentPageFromUri(this string uri)
    {
        var uriSegments = uri.Split('/');
        var currentPage = uriSegments[^1];
        return currentPage;
    }

    public static List<Claim> GetClaims(string jwtToken)
    {
        string payload = jwtToken.Split(".")[1];
        byte[] jsonBytes = payload.ParseBase64StringWithoutPadding();
        var keyValuePairs = jsonBytes.FromBytes<Dictionary<string, object>>();
        var claims = keyValuePairs!.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!)).ToList();
        return claims;
    }

}
