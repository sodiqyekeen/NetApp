namespace NetApp.UI.Components;

public static class Identifier
{
    private static readonly Random _random = new();
    public static IdentifierContext SequentialContext() => new((n) => $"f{n:0000}");
    public static string NewId(int length = 8)
    {
        if (IdentifierContext.Current == null)
        {
            if (length > 16)
                length = 16;

            if (length <= 8)
                return $"f{_random.Next():x}";

            return $"f{_random.Next():x}{_random.Next():x}"[..length];
        }

        return IdentifierContext.Current.GenerateId();
    }

}
