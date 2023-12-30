namespace NetApp.UI.Components;

public class DialogResult
{
    protected internal DialogResult(object? data, bool cancelled)
    {
        Data = data;
        Cancelled = cancelled;
    }
    public object? Data { get; set; }
    public bool Cancelled { get; set; }
    public static DialogResult Ok<T>(T result) => new(result, false);
    public static DialogResult Cancel(object? data = null) => new(data ?? default, true);
}
