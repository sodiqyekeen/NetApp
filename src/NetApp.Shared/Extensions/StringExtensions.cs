using System.Security.Cryptography;
using System.Text;

namespace NetApp.Extensions;
public static class StringExtensions
{
    public static string RandomTokenString()
    {
        using var rngCryptoServiceProvider = RandomNumberGenerator.Create();
        var randomBytes = new byte[40];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    public static string GenerateRandomNumber(int length)
    {
        var numbers = "0123456789".ToCharArray();
        using var rngCryptoServiceProvider = RandomNumberGenerator.Create();

        var randomBytes = new byte[4 * length];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        var token = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            int index = BitConverter.ToUInt16(randomBytes, i * 4) % numbers.Length;
            token.Append(numbers[index]);
        }
        return token.ToString();
    }

    public static string GeneratePassword(int maxLength = 8)
    {
        string[] randomChars = new[] { "ABCDEFGHJKLMNOPQRSTUVWXYZ", "abcdefghijkmnopqrstuvwxyz", "0123456789", "@#$&" };
        var rand = new Random();
        var chars = new List<char>();

        //Add Uppercase
        chars.Insert(rand.Next(0, chars.Count), randomChars[0][rand.Next(0, randomChars[0].Length)]);

        //RequireLowercase
        chars.Insert(rand.Next(0, chars.Count), randomChars[1][rand.Next(0, randomChars[1].Length)]);

        //RequireDigit
        chars.Insert(rand.Next(0, chars.Count), randomChars[2][rand.Next(0, randomChars[2].Length)]);

        //opts.RequireNonAlphanumeric
        chars.Insert(rand.Next(0, chars.Count), randomChars[3][rand.Next(0, randomChars[3].Length)]);

        for (int i = chars.Count; i < maxLength || chars.Distinct().Count() < 6; i++)
        {
            string rcs = randomChars[rand.Next(0, randomChars.Length)];
            chars.Insert(rand.Next(0, chars.Count), rcs[rand.Next(0, rcs.Length)]);
        }

        return new string(chars.ToArray());
    }

}
